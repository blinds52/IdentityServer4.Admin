namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 用户 DTO
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 邮件
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 用户拥有的角色
        /// </summary>
        public string Roles { get; set; }
        
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}