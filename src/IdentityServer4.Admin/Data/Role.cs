using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Data
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role<TKey> : IdentityRole<TKey>, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class Role : Role<int>
    {
    }
}