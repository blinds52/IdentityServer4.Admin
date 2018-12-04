using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class DashboardController : ApiControllerBase
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public DashboardController(ConfigurationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] PaginationQuery input)
        {
            var output = new DashboardDto();
            output.ApiResourceCount = await _dbContext.ApiResources.CountAsync();
            output.ClientCount = await _dbContext.Clients.CountAsync();
            output.LockedUserCount = await _userManager.Users.CountAsync(u => u.LockoutEnd < DateTime.Now);
            output.UserCount = await _userManager.Users.CountAsync();
            return new ApiResult(output);
        }
    }
}