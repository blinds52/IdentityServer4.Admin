using System;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/user")]
    [Authorize]
    [SecurityHeaders]
    [AllowAnonymous]
    public class PermissionCheckController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDbContext _dbContext;

        public PermissionCheckController(UserManager<User> userManager, IDbContext dbContext,
            ILoggerFactory loggerFactory) : base( loggerFactory)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpHead("{userId}/permission/{permission}")]
        public IActionResult IsGrantAsync(Guid userId, string permission)
        {
//            //TODO: 优化性能
//            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
//            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
//            if (!(HttpContext.User.Identity.Name == AdminConsts.AdminName ||
//                  user.UserName == HttpContext.User.Identity.Name))
//                return new ApiResult(ApiResult.Error, "禁止访问");
//            var isGrant =
//                await _dbContext.UserPermissions.AnyAsync(up => up.UserId == userId && up.Permission.Name == permission);
//            if (!isGrant)
//            {
//                var roles = _dbContext.UserRoles.Where(ur => ur.UserId == userId);
//                foreach (var role in roles)
//                {
//                    isGrant = await _dbContext.RolePermissions.AnyAsync(rp =>
//                        rp.RoleId == role.RoleId && rp.Permission == permission);
//                    if (isGrant) break;
//                }
//            }
//
//            if (isGrant) return new OkResult();
            return new NotFoundResult();
        }
    }
}