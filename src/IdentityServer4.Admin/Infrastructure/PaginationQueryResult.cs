using System.Collections.Generic;

namespace IdentityServer4.Admin.Infrastructure
{
    public class PaginationQueryResult<TEntity>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public List<TEntity> Result { get; set; }
    }

    public class PaginationQueryResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public object Result { get; set; }
    }
}