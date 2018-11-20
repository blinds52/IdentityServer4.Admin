using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Data
{
    public class User : IdentityUser, IEntity<string>
    {
    }
}