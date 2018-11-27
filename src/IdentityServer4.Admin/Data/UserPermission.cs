using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Data
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class UserPermission : IEntity<int>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key] 
        public int Id { get; set; }
        
        /// <summary>
        /// 用户编号
        /// </summary>
        [Required]
        public int UserId { get; set; }
                        
        /// <summary>
        /// 权限编号
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Permission { get; set; }     
    }
}