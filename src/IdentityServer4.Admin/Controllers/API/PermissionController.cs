using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class PermissionController : ApiControllerBase
    {
        private readonly IRepository<Permission, Guid> _permissionRepository;
        private readonly IRepository<RolePermission, Guid> _rolePermissionRepository;
        //private readonly IRepository<UserPermission, Guid> _userPermissionRepository;

        public PermissionController(IRepository<Permission, Guid> permissionRepository,
            IRepository<RolePermission, Guid> rolePermissionRepository,
            //IRepository<UserPermission, Guid> userPermissionRepository,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory) : base(unitOfWork, loggerFactory)
        {
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            //_userPermissionRepository = userPermissionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionDto dto)
        {
            dto.Name = dto.Name.Trim();
            dto.Description = dto.Description?.Trim();

            if (dto.Name == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"权限名不能是: {AdminConsts.AdminName}");
            if (await _permissionRepository.FirstOrDefaultAsync(u => u.Name == dto.Name) != null)
                return new ApiResult(ApiResult.Error, "权限已经存在");

            var permission = new Permission
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _permissionRepository.InsertAsync(permission);
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }

        [HttpGet]
        public IActionResult Find([FromQuery] PaginationQuery input)
        {
            var queryResult = _permissionRepository.PagedQuery(input);
            var permissionDtos = new List<PermissionDto>();
            foreach (var permission in queryResult.Result)
            {
                permissionDtos.Add(new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description,
                });
            }

            return new ApiResult(queryResult.ToResult(permissionDtos));
        }

        [HttpGet("{permissionId}")]
        public async Task<IActionResult> FindFirst(Guid permissionId)
        {
            var permission = await _permissionRepository.GetAsync(permissionId);
            var dto = new PermissionDto();
            if (permission != null)
            {
                dto.Name = permission.Name;
                dto.Description = permission.Description;
            }

            return new ApiResult(dto);
        }

        [HttpPut("{permissionId}")]
        public async Task<IActionResult> Update(Guid permissionId, [FromBody] UpdatePermissionDto dto)
        {
            dto.Name = dto.Name.Trim();
            dto.Description = dto.Description?.Trim();

            if (dto.Name == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"权限名不能是: {AdminConsts.AdminName}");

            var permission = await _permissionRepository.GetAsync(permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            if (await _permissionRepository.FirstOrDefaultAsync(u => u.Name == dto.Name) != null)
                return new ApiResult(ApiResult.Error, "权限已经存在");

            permission.Name = dto.Name;
            permission.Description = dto.Description;
            await _permissionRepository.UpdateAsync(permission);
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> Delete(Guid permissionId)
        {
            var permission = await _permissionRepository.GetAsync(permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            if (permission.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "不能删除管理员权限");
            await _rolePermissionRepository.DeleteAsync(rp => rp.Permission == permission.Name);
           // await _userPermissionRepository.DeleteAsync(rp => rp.Permission == permission.Name);

            await _permissionRepository.DeleteAsync(permission);
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }
    }
}