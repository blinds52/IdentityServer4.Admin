using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    public class ChangePasswordDto
    {
        [StringLength(24)]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}