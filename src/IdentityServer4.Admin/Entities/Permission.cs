using System;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure.Entity;

namespace IdentityServer4.Admin.Entities
{
    public class Permission : EntityBase<Guid>
    {
        /// <summary>
        /// Unique name of the permission.
        /// This is the key name to grant permissions.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// A brief description for this permission.
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"[Permission: {Name}]";
        }
    }
}