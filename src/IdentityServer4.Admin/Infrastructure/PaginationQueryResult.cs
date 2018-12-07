using System.Collections.Generic;

namespace IdentityServer4.Admin.Infrastructure
{
    public class PaginationQueryResult<TEntity>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public List<TEntity> Result { get; set; }

        public PaginationQueryResult ToResult(object dtoResult)
        {
            return new PaginationQueryResult
            {
                Total = Total,
                Page = Page,
                Size = Size,
                Result = dtoResult
            };
        }
    }

    public class PaginationQueryResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public object Result { get; set; }
    }
}