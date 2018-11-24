using System.Collections.Generic;
using System.Linq;
using System.Net;
using IdentityServer4.Admin.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ModelState.IsValid)
            {
                context.Result = new ApiResult(ApiResult.ModelNotValid,
                    ModelState.First().Value.Errors.First().ErrorMessage);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}