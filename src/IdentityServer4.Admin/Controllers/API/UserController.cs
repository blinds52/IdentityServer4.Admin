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
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly RoleManager<Role> _roleManager;

        public UserController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IDbContext dbContext,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory) : base(loggerFactory)
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
            dto.FirstName = dto.FirstName?.Trim();
            dto.LastName = dto.LastName?.Trim();
            dto.Title = dto.Title?.Trim();
            dto.Level = dto.Level?.Trim();
            dto.Group = dto.Group?.Trim();
            dto.OfficePhone = dto.OfficePhone?.Trim();
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
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Title = dto.Title,
                Level = dto.Level,
                Group = dto.Group,
                OfficePhone = dto.OfficePhone,
                Sex = dto.Sex
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
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Title = user.Title,
                    Level = user.Level,
                    Group = user.Group,
                    OfficePhone = user.OfficePhone,
                    Sex = user.Sex,
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
                dto.FirstName = user.FirstName;
                dto.LastName = user.LastName;
                dto.Title = user.Title;
                dto.Level = user.Level;
                dto.Group = user.Group;
                dto.OfficePhone = user.OfficePhone;
                dto.Sex = user.Sex;
                dto.Roles = string.Join(",", await _userManager.GetRolesAsync(user));
            }

            return new ApiResult(dto);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync(Guid userId, [FromBody] UpdateUserDto dto)
        {
            dto.Email = dto.Email.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.UserName = dto.UserName.Trim();
            dto.FirstName = dto.FirstName?.Trim();
            dto.LastName = dto.LastName?.Trim();
            dto.Title = dto.Title?.Trim();
            dto.Level = dto.Level?.Trim();
            dto.Group = dto.Group?.Trim();
            dto.OfficePhone = dto.OfficePhone?.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName && dto.UserName != AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户名不能修改");

            if (user.UserName != AdminConsts.AdminName && dto.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"用户名不能是 {AdminConsts.AdminName}");

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Title = dto.Title;
            user.Level = dto.Level;
            user.Group = dto.Group;
            user.OfficePhone = dto.OfficePhone;
            user.Sex = dto.Sex;

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

        #region User Permission	

        [HttpGet("{userId}/permission")]
        public async Task<IActionResult> FindUserPermission(Guid userId, PaginationQuery query)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");

            var queryResult = _dbContext.UserRoles.Where(ur => ur.UserId == userId)
                .Join(_dbContext.Roles, userRole => userRole.RoleId, role => role.Id,
                    (userRole, role) => new {RoleId = role.Id, RoleName = role.Name})
                .Join(_dbContext.RolePermissions, role => role.RoleId, rolePermission => rolePermission.RoleId,
                    (role, rolePermission) => new {role.RoleId, role.RoleName, rolePermission.PermissionId})
                .Join(_dbContext.Permissions, rolePermission => rolePermission.PermissionId,
                    permission => permission.Id,
                    (rolePermission, permission) => new
                    {
                        rolePermission.RoleId, rolePermission.RoleName, rolePermission.PermissionId, permission.Name,
                        permission.Description
                    }).PagedQuery(query);

            return new ApiResult(queryResult);
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

            var roles = _dbContext.UserRoles.Where(ur => ur.UserId == userId).Join(_dbContext.Roles, ur => ur.RoleId,
                    r => r.Id, (ur, r) =>
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