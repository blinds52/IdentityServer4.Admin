using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.WsFederation;
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

        [StringLength(256)] 
        public string FirstName { get; set; }

        [StringLength(256)] 
        public string LastName { get; set; }

        public Sex Sex { get; set; }

        [Phone] 
        [StringLength(256)]
        public string OfficePhone { get; set; }
    }
}