using System;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure.Entity;

namespace IdentityServer4.Admin.Entities
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RolePermission : EntityBase<Guid>
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }

        /// <summary>
        /// 权限编号
        /// </summary>
        [Required]
        public Guid PermissionId { get; set; }
    }
}