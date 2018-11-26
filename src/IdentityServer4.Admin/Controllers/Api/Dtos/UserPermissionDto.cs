namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    /// <summary>
    /// 用户权限 DTO
    /// </summary>
    public class UserPermissionDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Permission { get; set; }
    }
}