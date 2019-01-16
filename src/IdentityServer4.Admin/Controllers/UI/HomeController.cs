using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{
    public class HomeController : BaseController
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirst("sub").Value;
            if (HttpContext.User.IsInRole(AdminConsts.AdminName))
            {
                return View();
            }

            return Redirect($"user/{userId}/profile");
        }
    }
}