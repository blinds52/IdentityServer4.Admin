using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Entities;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 更新用户 DTO
    /// </summary>
    public class UpdateProfileInputDto
    {
        /// <summary>
        /// 邮件
        /// </summary>
        [StringLength(50)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// 姓
        /// </summary>
        [StringLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [StringLength(50)]
        public string LastName { get; set; }               

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