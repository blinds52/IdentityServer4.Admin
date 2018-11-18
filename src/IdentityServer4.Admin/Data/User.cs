using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;

namespace IdentityServer4.Admin.Data
{
    public class User : IdentityUser, IEntity<string>
    {
    }
}