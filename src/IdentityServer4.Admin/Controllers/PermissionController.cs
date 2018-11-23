using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4.Admin.Controllers
{ 
    [Authorize()]
    public class PermissionController
    {
        public Task<bool>
    }
}