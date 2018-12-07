using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Infrastructure.Entity
{
    /// <summary>
    /// 实体数据存储操作类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">实体主键类型</typeparam>
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly DbSet<TEntity> _set;
        private readonly DbContext _dbContext;

        public virtual DbConnection Connection
        {
            get
            {
                var connection = _dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return connection;
            }
        }

        public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public Repository(IDbContext dbContext)
        {
            _dbContext = (DbContext) dbContext;
            _set = _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return GetAllIncluding();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = Table.AsQueryable();

            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }

            return query;
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public TEntity Get(TKey id)
        {
            return GetAll().FirstOrDefault(i => i.Id.Equals(id));
        }

        public async Task<TEntity> GetAsync(TKey id)
        {
            return await GetAll().FirstOrDefaultAsync(i => i.Id.Equals(id));
        }

        public TEntity Insert(TEntity entity)
        {
            return Table.Add(entity).Entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            return (await Table.AddAsync(entity)).Entity;
        }

        public TKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);

            if (entity.IsTransient())
            {
                _dbContext.SaveChanges();
            }

            return entity.Id;
        }

        public async Task<TKey> InsertAndGetIdAsync(TEntity entity)
        {
            entity = await InsertAsync(entity);

            if (entity.IsTransient())
            {
                await _dbContext.SaveChangesAsync();
            }

            return entity.Id;
        }

        public TEntity InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                ? Insert(entity)
                : Update(entity);
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return entity.IsTransient()
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        public TKey InsertOrUpdateAndGetId(TEntity entity)
        {
            return InsertOrUpdate(entity).Id;
            ;
        }

        public Task<TKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdateAndGetId(entity));
        }

        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return entity;;
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity = Update(entity);
            return Task.FromResult(entity);
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.FromResult(0);
        }

        public void Delete(TKey id)
        {
            var entity = GetFromChangeTrackerOrNull(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }

            entity = Get(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }

            //Could not found the entity, do nothing.
        }

        public Task DeleteAsync(TKey id)
        {
            Delete(id);
            return Task.FromResult(0);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
            return Task.FromResult(0);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }

        public PaginationQueryResult<TEntity> PagedQuery<TOrderByColumn>(
            PaginationQuery input,
            Expression<Func<TEntity, bool>> where = null,
            Expression<Func<TEntity, TOrderByColumn>> orderBy = null)  
        {
            var result = new PaginationQueryResult<TEntity>();
            var entities = Table.AsQueryable();
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

        public PaginationQueryResult<TEntity> PagedQuery(
            PaginationQuery input,
            Expression<Func<TEntity, bool>> where = null)  
        {
            var result = new PaginationQueryResult<TEntity>();
            var entities = Table.AsQueryable();
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

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }
            Table.Attach(entity);
        }
        
        private TEntity GetFromChangeTrackerOrNull(TKey id)
        {
            var entry = _dbContext.ChangeTracker.Entries()
                .FirstOrDefault(
                    ent =>
                        ent.Entity is TEntity &&
                        EqualityComparer<TKey>.Default.Equals(id, (ent.Entity as TEntity).Id)
                );

            return entry?.Entity as TEntity;
        }
    }
}