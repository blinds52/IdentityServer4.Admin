using System;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure.Entity;

namespace IdentityServer4.Admin.Entities
{
    public class UserPermission : ICreationAudited
    {
        /// <summary>
        /// 用户权限索引: md5(USERID_PERMISSION)
        /// </summary>
        [Key]
        [StringLength(256)]
        public string Permission { get; set; }

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        public string CreatorUserId { get; set; }
        
        public override bool Equals(object obj)
        {
            var other = obj as UserPermission;
            if (other == null)
            {
                return false;
            }

            return Permission.Equals(other.Permission);
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        /// 当前 <see cref="T:System.Object"/> 的哈希代码。<br/>
        /// 如果<c>Id</c>为<c>null</c>则返回0，
        /// 如果不为<c>null</c>则返回<c>Id</c>对应的哈希值
        /// </returns>
        public override int GetHashCode()
        {
            if (Permission == null)
            {
                return 0;
            }

            return Permission.GetHashCode();
        }
    }
}