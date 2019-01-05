using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class PagedQueryUserDto : PagedQuery
    {
        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(200)]
        public string Titles { get; set; }

        /// <summary>
        /// 团队
        /// </summary>
        [StringLength(50)]
        public string Group { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [StringLength(10)]
        public string Q { get; set; }
    }
}