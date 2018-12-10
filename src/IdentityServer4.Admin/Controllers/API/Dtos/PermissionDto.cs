using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 权限 DTO
    /// </summary>
    public class PermissionDto
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}