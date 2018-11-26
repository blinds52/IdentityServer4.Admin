using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace IdentityServer4.Admin.Data
{
    public class Permission : IEntity<string>
    {
        /// <summary>
        /// Unique name of the permission.
        /// This is the key name to grant permissions.
        /// </summary>
        [Required]
        [StringLength(255)]
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

        [Key] 
        [StringLength(50)]
        public string Id { get; set; }
    }
}