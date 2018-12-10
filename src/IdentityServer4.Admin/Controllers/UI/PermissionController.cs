using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{ 
    [Authorize(Roles = AdminConsts.AdminName)]
    public class PermissionController : BaseController
    { 
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("[controller]/{permissionId}/edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}