using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.Api.Dtos;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class PermissionController : ApiControllerBase
    {
        private readonly AdminDbContext _dbContext;

        public PermissionController(AdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> Delete(string permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null)
            {
                return new ApiResult(ApiResult.Error, "权限不存在");
            }

            _dbContext.Permissions.Remove(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet("{permissionId}")]
        public async Task<IActionResult> GetFirst(string permissionId)
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
        public async Task<IActionResult> Update(string permissionId, [FromBody] UpdatePermissionDto dto)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(u => u.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");
            permission.Name = dto.Name;
            permission.Description = dto.Description;
            _dbContext.Permissions.Update(permission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionDto dto)
        {
            if (await _dbContext.Permissions.AnyAsync(u => u.Name == dto.Name))
            {
                return new ApiResult(ApiResult.Error, "权限已经存在");
            }

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
        public IActionResult Query([FromQuery] PaginationQuery input)
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
    }
}