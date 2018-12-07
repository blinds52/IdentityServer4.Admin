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
    public class RoleController : ApiControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<RolePermission, Guid> _rolePermissionRepository;
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IRepository<Permission, Guid> _permissionRepository;

        public RoleController(RoleManager<Role> roleManager,
            IRepository<RolePermission, Guid> rolePermissionRepository,
            IRepository<Role, Guid> roleRepository,
            IRepository<Permission, Guid> permissionRepository,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory) : base(unitOfWork, loggerFactory)
        {
            _roleManager = roleManager;
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
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
            var queryResult = _roleRepository.PagedQuery(input);
            var roleDtos = new List<RoleDto>();
            foreach (var role in queryResult.Result)
            {
                roleDtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                });
            }

            return new ApiResult(queryResult.ToResult(roleDtos));
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> FindFirst(Guid roleId)
        {
            var role = await _roleRepository.GetAsync(roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            var dto = new RoleDto {Name = role.Name, Description = role.Description};
            return new ApiResult(dto);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(Guid roleId, [FromBody] UpdateRoleDto dto)
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
        public async Task<IActionResult> Delete(Guid roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许删除");

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region Role Permission

        [HttpPost("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> CreateRolePermission(Guid roleId, Guid permissionId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");
            if (role.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员角色不允许修改");
            var permission = await _permissionRepository.GetAsync(permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");

            if (await _rolePermissionRepository.FirstOrDefaultAsync(rp =>
                    rp.RoleId == roleId && rp.PermissionId == permissionId) != null)
                return new ApiResult(ApiResult.Error, "权限已经存在");

            await _rolePermissionRepository.InsertAsync(new RolePermission
            {
                Permission = permission.Name,
                PermissionId = permissionId,
                RoleId = roleId
            });
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }

        [HttpGet("{roleId}/permission")]
        public IActionResult FindRolePermission([FromQuery] PaginationQuery input)
        {
            var queryResult = _rolePermissionRepository.PagedQuery(input);
            var rolePermissionDtos = new List<RolePermissionDto>();
            foreach (var rolePermission in queryResult.Result)
            {
                rolePermissionDtos.Add(new RolePermissionDto
                {
                    Id = rolePermission.Id,
                    Permission = rolePermission.Permission,
                    RoleId = rolePermission.RoleId,
                    PermissionId = rolePermission.PermissionId
                });
            }

            return new ApiResult(queryResult.ToResult(rolePermissionDtos));
        }

        [HttpDelete("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> DeleteRolePermission(Guid roleId, Guid permissionId)
        {
            var rolePermission = await _rolePermissionRepository.FirstOrDefaultAsync(p =>
                p.RoleId == roleId && p.PermissionId == permissionId);
            if (rolePermission == null) return new ApiResult(ApiResult.Error, "数据不存在");

           await _rolePermissionRepository.DeleteAsync(rolePermission);
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }

        #endregion
    }
}