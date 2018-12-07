using System;
using System.Linq.Expressions;
using IdentityServer4.Admin.Infrastructure.Entity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.Admin.Repositories
{
    public class ApiResourceRepository:  ExternalRepository<ApiResource, int>
    {
        public ApiResourceRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        protected override Expression<Func<ApiResource, int>> PrimaryKey => a => a.Id;
        
        protected override int GetPrimaryKey(ApiResource entity)
        {
            return entity.Id;
        }
    }
}