using System;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 角色权限 DTO
    /// </summary>
    public class RolePermissionOutputDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        public Guid RoleId { get; set; }
        
        /// <summary>
        /// 权限编号
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Permission { get; set; }
        
        /// <summary>
        /// 权限描述
        /// </summary>
        public string PermissionDescription { get; set; }
    }
}