using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Common
{
    internal class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 201;
            var info = new ApiResult(ApiResult.InternalError, GetInnerMessage(context.Exception));
            _logger.LogError(context.Exception.ToString());
            context.Result = new JsonResult(info);
        }

        private string GetInnerMessage(Exception ex)
        {
            return ex.InnerException != null ? GetInnerMessage(ex.InnerException) : ex.Message;
        }
    }
}