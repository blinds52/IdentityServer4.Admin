using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class PermissionController : ApiControllerBase
    {
        private readonly AdminDbContext _dbContext;

        public PermissionController(AdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionDto dto)
        {
            if (dto.Name?.Trim() == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"权限名不能是: {AdminConsts.AdminName}");
            if (await _dbContext.Permissions.AnyAsync(u => u.Name == dto.Name))
                return new ApiResult(ApiResult.Error, "权限已经存在");

            var permission = new Permission
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _dbContext.Permissions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet]
        public IActionResult Find([FromQuery] PaginationQuery input)
        {
            var output = _dbContext.Permissions.PageList(input);
            var permissions = (IEnumerable<Permission>) output.Result;
            var permissionDtos = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                permissionDtos.Add(new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description,
                });
            }

            output.Result = permissionDtos;
            return new ApiResult(output);
        }

        [HttpGet("{permissionId}")]
        public async Task<IActionResult> FindFirst(int permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            var dto = new PermissionDto();
            if (permission != null)
            {
                dto.Name = permission.Name;
                dto.Description = permission.Description;
            }

            return new ApiResult(dto);
        }

        [HttpPut("{permissionId}")]
        public async Task<IActionResult> Update(int permissionId, [FromBody] UpdatePermissionDto dto)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(u => u.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            if (await _dbContext.Permissions.AnyAsync(u => u.Name == dto.Name))
                return new ApiResult(ApiResult.Error, "权限已经存在");

            permission.Name = dto.Name;
            permission.Description = dto.Description;
            _dbContext.Permissions.Update(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> Delete(int permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            _dbContext.RolePermissions.RemoveRange(
                _dbContext.RolePermissions.Where(rp => rp.Permission == permission.Name));
            _dbContext.UserPermissions.RemoveRange(
                _dbContext.UserPermissions.Where(rp => rp.Permission == permission.Name));
            _dbContext.Permissions.Remove(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }
    }
}