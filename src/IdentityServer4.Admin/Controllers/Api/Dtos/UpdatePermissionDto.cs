using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    /// <summary>
    /// 更新权限 DTO
    /// </summary>
    public class UpdatePermissionDto
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
    }
}