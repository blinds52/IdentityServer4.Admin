using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Data
{
    public class Permission : IEntity<int>
    {
        /// <summary>
        /// Unique name of the permission.
        /// This is the key name to grant permissions.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the permission.
        /// This can be used to show permission to the user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A brief description for this permission.
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return $"[Permission: {Name}]";
        }
        
        [Key]
        public int Id { get; set; }
    }
}