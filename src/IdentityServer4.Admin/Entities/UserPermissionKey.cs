using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Entities
{
    public class UserPermissionKey
    {
        /// <summary>
        /// 用户权限索引: md5(USERID_PERMISSION)
        /// </summary>
        [Key] 
        public string PermissionKey { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as UserPermissionKey;
            if (other == null)
            {
                return false;
            }

            return PermissionKey.Equals(other.PermissionKey);
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
            if (PermissionKey == null)
            {
                return 0;
            }

            return PermissionKey.GetHashCode();
        }
    }
}