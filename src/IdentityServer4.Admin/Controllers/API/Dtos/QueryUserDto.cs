using IdentityServer4.Admin.Infrastructure;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class QueryUserDto : PaginationQuery
    {
        public string Keyword { get; set; }
    }
}