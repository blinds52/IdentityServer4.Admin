using IdentityServer4.Admin.Infrastructure;

namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class KeywordPagedQuery : PagedQuery
    {
        public string Keyword { get; set; }
    }
}