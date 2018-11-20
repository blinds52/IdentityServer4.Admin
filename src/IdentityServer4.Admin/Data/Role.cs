using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Data
{
    public class Role<TKey> : IdentityRole<TKey>, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public TKey ParentRoleId { get; set; }
    }

    public class Role : Role<string>
    {
    }
}