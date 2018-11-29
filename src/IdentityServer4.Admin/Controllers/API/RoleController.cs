using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class RoleController : ApiControllerBase
    {
        private readonly AdminDbContext _dbContext;
        private readonly RoleManager<Role> _roleManager;

        public RoleController(RoleManager<Role> roleManager, AdminDbContext dbContext)
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
        public async Task<IActionResult> Create([FromBody] RoleDto dto)
        {
            dto.Name = dto.Name.Trim();
            dto.Description = dto.Description?.Trim();

            var newRole = new Role
            {
                Name = dto.Name,
                Description = dto.Description
            };
            var result = await _roleManager.CreateAsync(newRole);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpGet]
        public IActionResult Find([FromQuery] PaginationQuery input)
        {
            var output = _roleManager.Roles.PageList(input);
            var roles = (IEnumerable<Role>) output.Result;
            var roleDtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                roleDtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                });
            }

            output.Result = roleDtos;
            return new ApiResult(output);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> FindFirst(int roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            var dto = new RoleDto {Name = role.Name};
            return new ApiResult(dto);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(int roleId, [FromBody] UpdateRoleDto dto)
        {
            dto.Name = dto.Name.Trim();
            dto.Description = dto.Description?.Trim();

            if (dto.Name == "admin") return new ApiResult(ApiResult.Error, "角色不能使用 admin");

            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许修改");
            role.Name = dto.Name;
            role.Description = dto.Description;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(int roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许删除");
            _dbContext.UserRoles.RemoveRange(
                _dbContext.UserRoles.Where(rp => rp.RoleId == roleId));
            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region Role Permission

        [HttpPost("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> CreateRolePermission(int roleId, int permissionId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许修改");
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");

            if (await _dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId))
                return new ApiResult(ApiResult.Error, "权限已经存在");

            await _dbContext.RolePermissions.AddAsync(new RolePermission
            {
                Permission = permission.Name,
                PermissionId = permissionId,
                RoleId = roleId
            });
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet("{roleId}/permission")]
        public IActionResult FindRolePermission([FromQuery] PaginationQuery input)
        {
            var output = _dbContext.RolePermissions.PageList(input);
            var rolePermissions = (IEnumerable<RolePermission>) output.Result;
            var rolePermissionDtos = new List<RolePermissionDto>();
            foreach (var rolePermission in rolePermissions)
            {
                rolePermissionDtos.Add(new RolePermissionDto
                {
                    Id = rolePermission.Id,
                    Permission = rolePermission.Permission,
                    RoleId = rolePermission.RoleId,
                    PermissionId = rolePermission.PermissionId
                });
            }

            output.Result = rolePermissionDtos;
            return new ApiResult(output);
        }

        [HttpDelete("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> DeleteRolePermission(int roleId, int permissionId)
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