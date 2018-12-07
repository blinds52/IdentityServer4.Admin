using System;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 用户权限 DTO
    /// </summary>
    public class UserPermissionDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 权限编号
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Permission { get; set; }
    }
}