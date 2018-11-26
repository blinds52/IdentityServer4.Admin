using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        [MinLength(4)]
        public string UserName { get; set; }

        [StringLength(50)] [EmailAddress] public string Email { get; set; }

        [Required] [Phone] public string PhoneNumber { get; set; }
    }
}