using System.Net;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers
{
    [Authorize(Roles = "admin")]
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
    }
}