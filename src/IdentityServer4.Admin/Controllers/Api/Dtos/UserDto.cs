using System.Collections.Generic;

namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
    }
}