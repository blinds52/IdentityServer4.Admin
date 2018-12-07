using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure.Entity;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Repositories
{
    public class ClientRepository : ExternalRepository<Client, int>
    {
        public ClientRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        protected override Expression<Func<Client, int>> PrimaryKey => c => c.Id;
        
        protected override int GetPrimaryKey(Client entity)
        {
            return entity.Id;
        }
    }
}