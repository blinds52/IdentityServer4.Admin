using IdentityServer4.Admin.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{ 
    [Authorize(Roles = AdminConsts.AdminName)]
    [Route("[controller]")]
    public class PermissionController : Controller
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

        [HttpGet("{permissionId}/edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}