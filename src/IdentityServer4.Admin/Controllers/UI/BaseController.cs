using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{   
    public class BaseController : Controller
    {
        protected bool IsAdmin()
        {
            return User.IsInRole(AdminConsts.AdminName);
        }
    }
}