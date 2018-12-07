using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AdminDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly RoleManager<Role> _roleManager;

        public UserController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            AdminDbContext dbContext, IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory) : base(unitOfWork, loggerFactory)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _roleManager = roleManager;
        }

        #region User

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto dto)
        {
            dto.Password = dto.Password.Trim();
            dto.Email = dto.Email.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.UserName = dto.UserName.Trim();
            string normalizedName =
                _serviceProvider.ProtectPersonalData(_userManager.NormalizeKey(dto.UserName),
                    _userManager.Options);
            // NormalizedUserName 有唯一索引, 应该用它做查询
            if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedName && u.IsDeleted == false))
            {
                return new ApiResult(ApiResult.Error, "用户名已经存在");
            }

            var result = await _userManager.CreateAsync(new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            }, dto.Password);
            if (result.Succeeded)
            {
                return ApiResult.Ok;
            }

            return new ApiResult(ApiResult.Error,
                string.Join(",", result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        public async Task<IActionResult> FindAsync([FromQuery] QueryUserDto input)
        {
            PaginationQueryResult<User> queryResult;
            if (string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryResult = _userManager.Users.PagedQuery(input);
            }
            else
            {
                queryResult = _userManager.Users.PagedQuery(input,
                    u => u.Email.Contains(input.Keyword) || u.UserName.Contains(input.Keyword) ||
                         u.PhoneNumber.Contains(input.Keyword));
            }

            var userDtos = new List<UserDto>();
            foreach (var user in queryResult.Result)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsDelete = user.IsDeleted,
                    Roles = string.Join(",", await _userManager.GetRolesAsync(user))
                });
            }

            return new ApiResult(queryResult.ToResult(userDtos));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> FirstAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            var dto = new UserDto();
            if (user != null)
            {
                dto.PhoneNumber = user.PhoneNumber;
                dto.Email = user.Email;
                dto.UserName = user.UserName;
                dto.Id = user.Id;
                dto.Roles = string.Join(",", await _userManager.GetRolesAsync(user));
            }

            return new ApiResult(dto);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync(Guid userId, [FromBody] UpdateUserDto dto)
        {
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.Email = dto.Email.Trim();
            dto.UserName = dto.UserName.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName && dto.UserName != AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户名不能修改");

            if (user.UserName != AdminConsts.AdminName && dto.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"用户名不能是 {AdminConsts.AdminName}");

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");
            if (user.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户不能删除");
            user.LockoutEnabled = true;
            await _userManager.DeleteAsync(user);
            return ApiResult.Ok;
        }

        [HttpPut("{userId}/changePassword")]
        public async Task<IActionResult> ChangePasswordAsync(Guid userId, [FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");
            if (user.UserName == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员密码请通过 Profile 入口修改");
            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded) return new ApiResult(ApiResult.Error, result.Errors.First().Description);

            result = await _userManager.AddPasswordAsync(user, dto.NewPassword.Trim());
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region User Role

        [HttpPost("{userId}/role/{role}")]
        public async Task<IActionResult> CreateUserRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");

            if (await _userManager.IsInRoleAsync(user, AdminConsts.AdminName))
                return new ApiResult(ApiResult.Error, "管理员不能添加角色");

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded
                ? ApiResult.Ok
                : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpGet("{userId}/role")]
        public async Task<IActionResult> FindUserRoleAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");

            var roles = _dbContext.UserRoles.Join(_dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) =>
                    new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description
                    })
                .ToList();

            return new ApiResult(roles);
        }

        [HttpDelete("{userId}/role/{roleId}")]
        public async Task<IActionResult> DeleteUserRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return result.Succeeded
                ? ApiResult.Ok
                : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion
    }
}