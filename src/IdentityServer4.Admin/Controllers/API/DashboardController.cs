using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDbContext _dbContext;

        public DashboardController(IDbContext dbContext,
            ILoggerFactory loggerFactory) : base( loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var output = new DashboardDto
            {
                ApiResourceCount = await _dbContext.ApiResources.CountAsync(),
                ClientCount = await _dbContext.Clients.CountAsync(),
                LockedUserCount = await _dbContext.Users.CountAsync(u => u.LockoutEnd < DateTime.Now),
                UserCount = await _dbContext.Users.CountAsync()
            };
            return new ApiResult(output);
        }
    }
}