using IdentityServer4.Admin.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{
    [Authorize(Roles = AdminConsts.AdminName)]
    [Route("[controller]")]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("{userId}/edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet("{userId}/changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet("{userId}/role")]
        public IActionResult Role()
        {
            return View();
        }

        [HttpGet("{userId}/permission")]
        public IActionResult Permission()
        {
            return View();
        }
    }
}