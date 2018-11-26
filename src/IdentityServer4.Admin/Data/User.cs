using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Data
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : IdentityUser<int>, IEntity<int>
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}