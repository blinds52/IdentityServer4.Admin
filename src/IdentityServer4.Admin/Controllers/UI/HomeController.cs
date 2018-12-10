using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.UI
{
    public class HomeController : BaseController
    {
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

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
                return Redirect("User");
            }

            return Redirect($"User/{userId}/profile");
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        public IActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}