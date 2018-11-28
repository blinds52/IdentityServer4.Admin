using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 更新用户 DTO
    /// </summary>
    public class UpdateProfileDto
    {
        /// <summary>
        /// 邮件
        /// </summary>
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}