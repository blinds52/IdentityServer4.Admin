using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/api-resource")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class ApiResourceController : ApiControllerBase
    {
        private readonly IDbContext _dbContext;

        public ApiResourceController(IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateApiResourceDto dto)
        {
            if (await _dbContext.ApiResources.AnyAsync(u => u.Name == dto.Name))
            {
                return new ApiResult(ApiResult.Error, "资源名已经存在");
            }

            var apiResource = new ApiResource();
            apiResource.Enabled = dto.Enabled;
            apiResource.Name = dto.Name;
            apiResource.DisplayName = dto.DisplayName;
            apiResource.Description = dto.Description;
            apiResource.UserClaims = dto.UserClaims;

            await _dbContext.ApiResources.AddAsync(apiResource.ToEntity());
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAsync([FromQuery] PagedQuery input)
        {
            var output = await _dbContext.ApiResources.PagedQuery(input);
            return new ApiResult(output);
        }

        [HttpDelete("{apiResourceId}")]
        public async Task<IActionResult> DeleteAsync(int apiResourceId)
        {
            var apiResource = await _dbContext.ApiResources.FirstOrDefaultAsync(u => u.Id == apiResourceId);
            if (apiResource == null) return new ApiResult(ApiResult.Error, "API 资源不存在或已经删除");

            //TODO: 确认其它表数据一并删除
            _dbContext.ApiResources.Remove(apiResource);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }
    }
}