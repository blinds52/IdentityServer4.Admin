using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    /// <summary>
    /// 更新角色 DTO
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        
        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
    }
}