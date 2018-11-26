namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    /// <summary>
    /// 角色权限 DTO
    /// </summary>
    public class RolePermissionDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Permission { get; set; }
    }
}