using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using IdentityServer4.Admin.Repositories;
using IdentityServer4.EntityFramework.DbContexts;
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
        private readonly  ApiResourceRepository _apiResourceRepository;

        public ApiResourceController( ApiResourceRepository apiResourceRepository, IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory) : base(unitOfWork, loggerFactory)
        {
            _apiResourceRepository = apiResourceRepository;
        }

        [HttpGet]
        public IActionResult Query([FromQuery] PaginationQuery input)
        {
            var output = _apiResourceRepository.PagedQuery(input);
            return new ApiResult(output);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiResource resource)
        {
            await _apiResourceRepository.InsertAsync(resource.ToEntity());
            await UnitOfWork.CommitAsync();
            return ApiResult.Ok;
        }
    }
}