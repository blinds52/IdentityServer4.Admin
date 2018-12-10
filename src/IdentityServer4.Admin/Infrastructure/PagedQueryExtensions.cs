using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IdentityServer4.Admin.Infrastructure.Entity;

namespace IdentityServer4.Admin.Infrastructure
{
    public static class PagedQueryExtensions
    {
        public static PaginationQueryResult<TEntity> PagedQuery<TEntity, TKey, TOrderBy>(this IQueryable<TEntity> dbSet,
            PaginationQuery input,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, TOrderBy>> orderBy = null) where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var result = new PaginationQueryResult<TEntity>();
            var entities = dbSet.AsQueryable();
            if (where != null)
            {
                entities = entities.Where(where);
            }

            result.Total = entities.Count();
            result.Page = input.Page ?? 1;
            result.Size = input.Size ?? 20;

            if (orderBy == null)
            {
                if (input.SortByDesc)
                {
                    entities = entities.OrderByDescending(e => e.Id).Skip((result.Page - 1) * result.Size)
                        .Take(result.Size);
                }
                else
                {
                    entities = entities.Skip((result.Page - 1) * result.Size).Take(result.Size);
                }
            }
            else
            {
                if (input.SortByDesc)
                {
                    entities = entities.OrderByDescending(orderBy).Skip((result.Page - 1) * result.Size)
                        .Take(result.Size);
                }
                else
                {
                    entities = entities.OrderBy(orderBy).Skip((result.Page - 1) * result.Size)
                        .Take(result.Size);
                }
            }


            result.Result = result.Total == 0 ? new List<TEntity>() : entities.ToList();
            return result;
        }

        public static PaginationQueryResult<TEntity> PagedQuery<TEntity>(this IQueryable<TEntity> dbSet,
            PaginationQuery input,
            Expression<Func<TEntity, bool>> where = null) where TEntity : class
        {
            var result = new PaginationQueryResult<TEntity>();
            var entities = dbSet.AsQueryable();
            if (where != null)
            {
                entities = entities.Where(where);
            }

            result.Total = entities.Count();
            result.Page = input.Page ?? 1;
            result.Size = input.Size ?? 20;

            entities = entities.Skip((result.Page - 1) * result.Size).Take(result.Size);

            result.Result = result.Total == 0 ? new List<TEntity>() : entities.ToList();
            return result;
        }
    }
}