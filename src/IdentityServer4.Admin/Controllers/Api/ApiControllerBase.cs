using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers.Api
{
    public class ApiControllerBase : Controller
    {
        protected string GetModelStateErrorMsg()
        {
            var errors = new List<string>();
            foreach (var state in ModelState)
            {
                var error = state.Value.Errors.FirstOrDefault();
                if (error != null)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return string.Join(",", errors);
        }
    }
}