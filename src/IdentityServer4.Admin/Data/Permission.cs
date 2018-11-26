using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace IdentityServer4.Admin.Data
{
    public class Permission : IEntity<int>
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
        
        /// <summary>
        /// 主键
        /// </summary>
        [Key] 
        public int Id { get; set; }
    }
}