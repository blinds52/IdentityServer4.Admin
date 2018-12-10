using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Entities;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class CreateUserDto
    {        
        [Required]
        [StringLength(32)]
        [MinLength(4)]
        public string UserName { get; set; }
        
        [StringLength(256)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        
        [StringLength(24)]
        [MinLength(6)]
        [Required]
        public string Password { get; set; }
        
        [Required]
        [Phone]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// 姓
        /// </summary>
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }
        
        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(50)]
        public string Title { get; set; }
        
        /// <summary>
        /// 团队
        /// </summary>
        [StringLength(50)]
        public string Group { get; set; }
        
        /// <summary>
        /// 职级
        /// </summary>
        [StringLength(50)]
        public string Level { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        [Phone]
        [StringLength(50)]
        public string OfficePhone { get; set; }
    }
}