using IdentityServer4.Admin.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{
    [Authorize(Roles = AdminConsts.AdminName)]
    public class ApiResourceController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Create()
        {
            return View();
        }
    }
}