using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    /// <summary>
    /// 角色 DTO
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public int Id { get; set; }
        
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