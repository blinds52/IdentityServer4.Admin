using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class ApiResourceController : ApiControllerBase
    {
        private readonly ConfigurationDbContext _dbContext;

        public ApiResourceController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Query([FromQuery] PaginationQuery input)
        {
            var output = _dbContext.ApiResources.PageList(input);
            return new ApiResult(output);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiResource resource)
        {
            await _dbContext.ApiResources.AddAsync(resource.ToEntity());
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }
    }
}