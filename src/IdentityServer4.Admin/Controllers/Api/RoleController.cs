using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.Api.Dtos;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
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
            var newRole = new Role
            {
                Name = dto.Name
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
            foreach (var permission in roles)
            {
                roleDtos.Add(new RoleDto
                {
                    Id = permission.Id,
                    Name = permission.Name
                });
            }

            output.Result = roleDtos;
            return new ApiResult(output);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> FindFirst(int roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null)
            {
                return new ApiResult(ApiResult.Error, "角色不存在");
            }

            var dto = new RoleDto {Name = role.Name};
            return new ApiResult(dto);
        }
        
        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(int roleId, [FromBody] UpdateRoleDto dto)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            role.Name = dto.Name;
            role.Description = dto.Description;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(int roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null)
            {
                return new ApiResult(ApiResult.Error, "角色不存在");
            }

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region Role Permission

        [HttpPost("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> CreateRolePermission(int roleId, int permissionId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null)
            {
                return new ApiResult(ApiResult.Error, "角色不存在");
            }

            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null)
            {
                return new ApiResult(ApiResult.Error, "权限不存在");
            }

            await _dbContext.RolePermissions.AddAsync(new RolePermission
            {
                Permission = permission.Name,
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
                    RoleId = rolePermission.RoleId
                });
            }

            output.Result = rolePermissionDtos;
            return new ApiResult(output);
        }

        [HttpDelete("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> DeleteRolePermission(int roleId, int permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            var rolePermission = await _dbContext.RolePermissions.FirstOrDefaultAsync(p =>
                p.RoleId == roleId && p.Permission == permission.Name);
            if (rolePermission == null) return new ApiResult(ApiResult.Error, "数据不存在");

            _dbContext.RolePermissions.Remove(rolePermission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        #endregion
    }
}