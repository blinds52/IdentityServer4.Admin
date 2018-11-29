using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class CreateUserDto
    {        
        [Required]
        [StringLength(32)]
        [MinLength(4)]
        public string UserName { get; set; }
        
        [StringLength(256)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        
        [StringLength(24)]
        [MinLength(6)]
        [Required]
        public string Password { get; set; }
        
        [Required]
        [Phone]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
    }
}