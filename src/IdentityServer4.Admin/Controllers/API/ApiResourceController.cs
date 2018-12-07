using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
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
        private readonly AdminDbContext _dbContext;

        public ApiResourceController(AdminDbContext dbContext, IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory) : base(unitOfWork, loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Query([FromQuery] PaginationQuery input)
        {
            var output = _dbContext.ApiResources.PagedQuery(input);
            return new ApiResult(output);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiResource resource)
        {
            await _dbContext.ApiResources.AddAsync(resource.ToEntity());
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }
    }
}