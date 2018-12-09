using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class ApiResourceController : ApiControllerBase
    {
        private readonly IDbContext _dbContext;

        public ApiResourceController(IDbContext dbContext,  
            ILoggerFactory loggerFactory) : base( loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Find([FromQuery] PaginationQuery input)
        {
            var output = _dbContext.ApiResources.PagedQuery(input);
            return new ApiResult(output);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ApiResource resource)
        {
            await _dbContext.ApiResources.AddAsync(resource.ToEntity());
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }
    }
}