using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 更新用户 DTO
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(50)]
        [MinLength(4)]
        public string UserName { get; set; }

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
    }
}