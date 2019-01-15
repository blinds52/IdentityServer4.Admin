using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly RoleManager<Role> _roleManager;
        private readonly AdminOptions _options;

        public UserController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IDbContext dbContext,
            AdminOptions options,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _roleManager = roleManager;
            _options = options;
        }

        #region User

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto dto)
        {
            if (dto.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"用户名不能是 {AdminConsts.AdminName}");

            var user = Mapper.Map<User>(dto);

            // Check username exist
            string normalizedName =
                _serviceProvider.ProtectPersonalData(_userManager.NormalizeKey(user.UserName),
                    _userManager.Options);
            // NormalizedUserName 有唯一索引, 应该用它做查询
            if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedName && u.IsDeleted == false))
            {
                return new ApiResult(ApiResult.Error, "用户名已经存在");
            }

            // Check email exists
            if (await _userManager.Users.AnyAsync(u => u.Email == user.Email && u.IsDeleted == false))
            {
                return new ApiResult(ApiResult.Error, "邮箱已经存在");
            }

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                return ApiResult.Ok;
            }

            return new ApiResult(ApiResult.Error,
                string.Join(",", result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        public async Task<IActionResult> SearchAsync([FromQuery] KeywordPagedQuery input)
        {
            PagedQueryResult<User> queryResult;
            if (string.IsNullOrWhiteSpace(input.Q))
            {
                queryResult = await _userManager.Users.PagedQuery(input);
            }
            else
            {
                queryResult = await _userManager.Users.PagedQuery(input,
                    u => u.Email.Contains(input.Q) ||
                         u.UserName.Contains(input.Q) ||
                         u.PhoneNumber.Contains(input.Q) ||
                         // Comment: 如果不拼接成字符串报空引用错, Lewis Zou, 2018-12-10
                         $"{u.FirstName}{u.LastName}".Contains(input.Q)
                );
            }

            if (User.IsInRole(AdminConsts.AdminName))
            {
                var userDtos = new List<UserOutputDto>();
                foreach (var user in queryResult.Result)
                {
                    var dto = Mapper.Map<UserOutputDto>(user);
                    dto.Roles = string.Join("; ", await _userManager.GetRolesAsync(user));
                    userDtos.Add(dto);
                }

                return new ApiResult(queryResult.ToResult(userDtos));
            }
            else
            {
                var userDtos = new List<dynamic>();
                foreach (var user in queryResult.Result)
                {
                    userDtos.Add(new
                    {
                        user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        user.Email,
                        user.PhoneNumber,
                        user.Title
                    });
                }

                return new ApiResult(queryResult.ToResult(userDtos));
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> FirstAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var dto = Mapper.Map<UserOutputDto>(user);
                dto.Roles = string.Join("; ", await _userManager.GetRolesAsync(user));
                return new ApiResult(dto);
            }

            return new ApiResult(ApiResult.Error, "用户不存在");
        }


        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync(Guid userId, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");
            if (user.UserName == AdminConsts.AdminName && dto.UserName != AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "超级管理员用户名不能修改");

            if (user.UserName != AdminConsts.AdminName && dto.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"用户名不能是 {AdminConsts.AdminName}");

            string normalizedName =
                _serviceProvider.ProtectPersonalData(_userManager.NormalizeKey(user.UserName),
                    _userManager.Options);
            // NormalizedUserName 有唯一索引, 应该用它做查询
            if (await _userManager.Users.AnyAsync(u =>
                u.Id != userId && u.NormalizedUserName == normalizedName && u.IsDeleted == false))
            {
                return new ApiResult(ApiResult.Error, "用户名已经存在");
            }

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
                return new ApiResult(ApiResult.Error, "超级管理员不能删除");
            user.LockoutEnabled = true;
            await _userManager.DeleteAsync(user);
            return ApiResult.Ok;
        }


        [HttpPut("{userId}/changePassword")]
        public async Task<IActionResult> ChangePasswordAsync(Guid userId, [FromBody] ChangePasswordInputDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");
            if (user.UserName == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "超级管理员请通过个人信息入口修改密码");
            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded) return new ApiResult(ApiResult.Error, result.Errors.First().Description);

            result = await _userManager.AddPasswordAsync(user, dto.NewPassword.Trim());
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region User Permission	

        [HttpGet("{userId}/permission")]
        public async Task<IActionResult> FindUserPermissionAsync(Guid userId, PagedQuery query)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");

            if (user.UserName == AdminConsts.AdminName)
            {
                return new ApiResult(ApiResult.Error, "超级管理员不能添加角色");
            }

            // 添加用户权限记录，用于权限校验接口
            var permissions = _dbContext.RolePermissions.Join(_dbContext.Permissions, rp => rp.PermissionId,
                p => p.Id, (rp, p) => new {p.Name});
            foreach (var permission in permissions)
            {
                var key = $"{userId}_{permission}";
                await _dbContext.UserPermissionKeys.AddAsync(new UserPermission {Permission = key});
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded
                ? ApiResult.Ok
                : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }


        [HttpGet("{userId}/role")]
        public async Task<IActionResult> FindUserRoleAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");
            if (user.UserName == AdminConsts.AdminName)
            {
                return new ApiResult(ApiResult.Error, "超级管理员不能删除角色");
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");


            // 先删除用户权限记录
            var permissions = _dbContext.RolePermissions.Join(_dbContext.Permissions, rp => rp.PermissionId,
                p => p.Id, (rp, p) => new {p.Name});
            foreach (var permission in permissions)
            {
                var key = $"{userId}_{permission}";
                _dbContext.UserPermissionKeys.RemoveRange(
                    _dbContext.UserPermissionKeys.Where(up => up.Permission == key));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return result.Succeeded
                ? ApiResult.Ok
                : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion
    }
}