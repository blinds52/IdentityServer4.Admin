using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Common
{
    public class ApiResult : JsonResult
    {
        public const int Success = 200;
        public const int Error = 700;
        public const int ModelNotValid = 701;
        public const int DbError = 702;

        public static ApiResult Ok = new ApiResult();

        public ApiResult(object data = null) : base(new
        {
            Code = 200,
            Msg = "success",
            Data = data
        })
        {
        }

        public ApiResult(int code, string msg = null, object data = null) : base(new
        {
            Code = code,
            Msg = msg,
            Data = data
        })
        {
        }
    }
}