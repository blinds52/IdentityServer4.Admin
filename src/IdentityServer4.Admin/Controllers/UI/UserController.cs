using System;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Admin.Controllers.UI
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly AdminOptions _options;

        public UserController(IOptions<AdminOptions> options)
        {
            _options = options.Value;           
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("[controller]/create")]
        public IActionResult Create()
        {
            ViewData["Group"] = _options.Group;
            ViewData["Title"] = _options.Title;
            ViewData["Level"] = _options.Level;
            return View();
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("[controller]/{userId}/edit")]
        public IActionResult Edit()
        {
            ViewData["Group"] = _options.Group;
            ViewData["Title"] = _options.Title;
            ViewData["Level"] = _options.Level;
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
        public IActionResult Profile(Guid userId)
        {
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return RedirectToAction("AccessDenied", "Account");
            
            ViewData["Group"] = _options.Group;
            ViewData["Title"] = _options.Title;
            ViewData["Level"] = _options.Level;
            
            return View();
        }
    }
}