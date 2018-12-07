using System;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure.Entity;

namespace IdentityServer4.Admin.Entities
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class UserPermission : EntityBase<Guid>
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
                        
        /// <summary>
        /// 权限编号
        /// </summary>
        [Required]
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Permission { get; set; }  
    }
}