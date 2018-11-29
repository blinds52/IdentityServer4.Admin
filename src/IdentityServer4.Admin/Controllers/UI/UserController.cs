using IdentityServer4.Admin.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{
    [Authorize]
    public class UserController : BaseController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("[controller]/{userId}/edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("[controller]/{userId}/changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("[controller]/{userId}/role")]
        public IActionResult Role()
        {
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("[controller]/{userId}/permission")]
        public IActionResult Permission()
        {
            return View();
        }

        [HttpGet("[controller]/{userId}/profile")]
        public IActionResult Profile(int userId)
        {
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return RedirectToAction("AccessDenied", "Account");
            return View();
        }
    }
}