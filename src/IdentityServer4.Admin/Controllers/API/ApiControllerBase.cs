using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Admin.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer4.Admin.Controllers.API
{
    public class ApiControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ModelState.IsValid)
            {
                context.Result = new ApiResult(ApiResult.ModelNotValid,
                    ModelState.First(kv => kv.Value.ValidationState == ModelValidationState.Invalid).Value.Errors
                        .First().ErrorMessage);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}