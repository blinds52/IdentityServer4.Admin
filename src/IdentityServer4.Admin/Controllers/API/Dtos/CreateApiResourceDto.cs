using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class CreateApiResourceDto
    {        
        /// <summary>
        /// Indicates if this resource is enabled. Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        [StringLength(200)]
        [Required]
        public string Name { get; set; }
        
        [StringLength(200)]
        public string DisplayName { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; }
        
        /// <summary>
        /// List of accociated user claims that should be included when this resource is requested.
        /// </summary>
        public ICollection<string> UserClaims { get; set; } = new HashSet<string>();
    }
}