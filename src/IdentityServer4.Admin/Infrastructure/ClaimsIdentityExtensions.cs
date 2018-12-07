using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace IdentityServer4.Admin.Infrastructure
{
    /// <summary>
    /// <see cref="ClaimsIdentity"/>扩展操作类
    /// </summary>
    public static class ClaimsIdentityExtensions
    {
        /// <summary>
        /// 获取用户ID
        /// </summary>
        public static string GetUserId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}