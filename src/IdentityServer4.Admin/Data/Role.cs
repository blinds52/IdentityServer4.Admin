using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Data
{
    public class Role<TKey> : IdentityRole<TKey>, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        [StringLength(500)]
        public string Description { get; set; }                
    }

    public class Role : Role<string>
    {
    }
}