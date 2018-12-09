using System;
using IdentityServer4.Admin.Entities;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    /// <summary>
    /// 用户 DTO
    /// </summary>
    public class UserOutputDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid Id { get; set; }

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

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Title { get; set; }
        
        public string Group { get; set; }
        
        public string Level { get; set; }
        
        public string OfficePhone { get; set; }
        
        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; } 
        
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}