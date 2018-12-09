using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class PermissionController : ApiControllerBase
    {
        private readonly IDbContext _dbContext;

        public PermissionController(
            IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PermissionDto dto)
        {
            var permission = Mapper.Map<Permission>(dto);
            if (dto.Name == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"权限名不能是: {AdminConsts.AdminName}");
            if (await _dbContext.Permissions.FirstOrDefaultAsync(u => u.Name == dto.Name) != null)
                return new ApiResult(ApiResult.Error, "权限已经存在");

            await _dbContext.Permissions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet]
        public IActionResult Find([FromQuery] PaginationQuery input)
        {
            var queryResult = _dbContext.Permissions.PagedQuery(input);
            var dtos = Mapper.Map<List<PermissionDto>>(queryResult.Result);
            return new ApiResult(queryResult.ToResult(dtos));
        }

        [HttpGet("{permissionId}")]
        public async Task<IActionResult> FindFirstAsync(Guid permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            return permission != null ? new ApiResult(Mapper.Map<PermissionDto>(permission)) : new ApiResult();
        }

        [HttpPut("{permissionId}")]
        public async Task<IActionResult> UpdateAsync(Guid permissionId, [FromBody] PermissionDto dto)
        {
            if (dto.Name == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"权限名不能是: {AdminConsts.AdminName}");

            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            if (await _dbContext.Permissions.FirstOrDefaultAsync(u => u.Name == dto.Name
                                                                      && u.Id != permissionId) != null)
                return new ApiResult(ApiResult.Error, "权限已经存在");

            permission.Name = dto.Name;
            permission.Description = dto.Description;
            _dbContext.Permissions.Update(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> DeleteAsync(Guid permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            if (permission.Name == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "不能删除管理员权限");

            // 删除 RolePermission 映射
            _dbContext.RolePermissions.RemoveRange(
                _dbContext.RolePermissions.Where(rp => rp.PermissionId == permissionId));
            _dbContext.Permissions.Remove(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }
    }
}