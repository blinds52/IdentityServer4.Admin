namespace IdentityServer4.Admin.Common
{
    public class PaginationQueryResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public dynamic Result { get; set; }
    }
}