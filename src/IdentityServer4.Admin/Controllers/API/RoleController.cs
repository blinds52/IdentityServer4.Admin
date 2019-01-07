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
    [SecurityHeaders]
    public class RoleController : ApiControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IDbContext _dbContext;

        public RoleController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        #region Role

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="dto">角色 DTO</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RoleDto dto)
        {
            var role = Mapper.Map<Role>(dto);
            string normalizedName = _roleManager.NormalizeKey(role.Name);
            if (await _roleManager.Roles.AnyAsync(u => u.NormalizedName == normalizedName))
            {
                return new ApiResult(ApiResult.Error, "角色已经存在");
            }

            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAsync([FromQuery] PagedQuery input)
        {
            var queryResult = await _roleManager.Roles.PagedQuery(input);
            var dtos = Mapper.Map<List<RoleDto>>(queryResult.Result);
            return new ApiResult(queryResult.ToResult(dtos));
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> FindFirstAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            return new ApiResult(Mapper.Map<RoleDto>(role));
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateAsync(Guid roleId, [FromBody] RoleDto dto)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            if (role.Name == AdminConsts.AdminName && role.Name != AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员角色不能修改");

            if (role.Name != AdminConsts.AdminName && dto.Name == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"角色名不能是 {AdminConsts.AdminName}");

            string normalizedName = _roleManager.NormalizeKey(role.Name);

            if (await _roleManager.Roles.AnyAsync(u =>
                u.Id != roleId && u.NormalizedName == normalizedName))
            {
                return new ApiResult(ApiResult.Error, "角色名已经存在");
            }

            role.Name = dto.Name;
            role.Description = dto.Description;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许删除");

            // 删除所有用户角色映射
            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.Where(ur => ur.RoleId == roleId));
            // 删除所有角色权限映射
            _dbContext.RolePermissions.RemoveRange(_dbContext.RolePermissions.Where(rp => rp.RoleId == roleId));
            var result = await _roleManager.DeleteAsync(role);
            await _dbContext.SaveChangesAsync();
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region Role Permission

        [HttpPost("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> CreateRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许修改权限");
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");

            if (await _dbContext.RolePermissions.FirstOrDefaultAsync(rp =>
                    rp.RoleId == roleId && rp.PermissionId == permissionId) != null)
                return new ApiResult(ApiResult.Error, "权限已经存在");

            await _dbContext.RolePermissions.AddAsync(
                new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = roleId
                });
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet("{roleId}/permission")]
        public async Task<IActionResult> FindRolePermissionAsync(Guid roleId, [FromQuery] PagedQuery input)
        {
            var queryResult = await _dbContext.RolePermissions.Where(rp => rp.RoleId == roleId)
                .Join(_dbContext.Permissions,
                    rolePermission => rolePermission.PermissionId, permission => permission.Id,
                    (rolePermission, permission) => new RolePermissionOutputDto
                    {
                        Id = rolePermission.Id,
                        PermissionId = rolePermission.PermissionId,
                        RoleId = rolePermission.RoleId,
                        Permission = permission.Name,
                        PermissionDescription = permission.Description
                    }
                ).PagedQuery(input);

            return new ApiResult(queryResult);
        }

        [HttpDelete("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> DeleteRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            var rolePermission = await _dbContext.RolePermissions.FirstOrDefaultAsync(p =>
                p.RoleId == roleId && p.PermissionId == permissionId);
            if (rolePermission == null) return new ApiResult(ApiResult.Error, "数据不存在");

            _dbContext.RolePermissions.Remove(rolePermission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        #endregion
    }
}