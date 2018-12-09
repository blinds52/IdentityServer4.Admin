using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/user")]
    [Authorize]
    [SecurityHeaders]
    public class PermissionCheckController : ApiControllerBase
    {
        private readonly IDbContext _dbContext;

        public PermissionCheckController(IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpHead("{userId}/permission/{permission}")]
        public async Task<IActionResult> IsGrantAsync(Guid userId, string permission)
        {
            if (!(HttpContext.User.Identity.Name == AdminConsts.AdminName ||
                  HttpContext.User.Identity.Name == userId.ToString()))
                return new ApiResult(ApiResult.Error, "禁止访问");

            var key = $"{userId}_{permission}";

            var isGrant =
                await _dbContext.UserPermissionKeys.AnyAsync(up => up.PermissionKey == key);

            return isGrant ? (IActionResult) new OkResult() : new NotFoundResult();
        }
    }
}