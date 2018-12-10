using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4.Admin
{
    public class AdminDbContext : IdentityDbContext<User, Role, Guid>,
        IDbContext,
        IDesignTimeDbContextFactory<AdminDbContext>
    {
        private readonly ConfigurationStoreOptions _configurationStoreOptions;
        private readonly OperationalStoreOptions _operationalStoreOptions;
        private IDbContext _dbContextImplementation;

        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// 角色权限映射
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// 用户权限缓存
        /// </summary>
        public DbSet<UserPermission> UserPermissionKeys { get; set; }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        public DbSet<IdentityResource> IdentityResources { get; set; }

        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public DbSet<ApiResource> ApiResources { get; set; }

        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the device codes.
        /// </summary>
        /// <value>
        /// The device codes.
        /// </value>
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public AdminDbContext()
        {
        }

        public AdminDbContext(DbContextOptions<AdminDbContext> options,
            ConfigurationStoreOptions storeOptions,
            OperationalStoreOptions operationalStoreOptions)
            : base(options)
        {
            _configurationStoreOptions = storeOptions;
            _operationalStoreOptions = operationalStoreOptions;
        }

        public AdminDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AdminDbContext>();
            builder.UseSqlServer(GetConnectionString(args.Length > 0 ? args[0] : "appsettings.json"));

            return new AdminDbContext(builder.Options, new ConfigurationStoreOptions(), new OperationalStoreOptions());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureClientContext(_configurationStoreOptions);
            builder.ConfigureResourcesContext(_configurationStoreOptions);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions);

            builder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();
            builder.Entity<RolePermission>().HasIndex(p => new {p.PermissionId, p.RoleId}).IsUnique();
            builder.Entity<UserPermission>().HasIndex(p => p.Permission);

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = this.GetService<IHttpContextAccessor>()?.HttpContext?.User?.Identity?.GetUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyAbpConcepts(entry, userId);
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var userId = this.GetService<IHttpContextAccessor>()?.HttpContext?.User?.Identity?.GetUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyAbpConcepts(entry, userId);
            }

            return base.SaveChanges();
        }

        int IConfigurationDbContext.SaveChanges()
        {
            return SaveChanges();
        }

        Task<int> IConfigurationDbContext.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        int IPersistedGrantDbContext.SaveChanges()
        {
            return SaveChanges();
        }

        protected virtual void ApplyAbpConcepts(EntityEntry entry, string userId)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyAbpConceptsForAddedEntity(entry, userId);
                    break;
                case EntityState.Modified:
                    ApplyAbpConceptsForModifiedEntity(entry, userId);
                    break;
                case EntityState.Deleted:
                    ApplyAbpConceptsForDeletedEntity(entry, userId);
                    break;
            }
        }

        protected virtual void ApplyAbpConceptsForAddedEntity(EntityEntry entry, string userId)
        {
            CheckAndSetId(entry);

            var creationAudited = entry.Entity as ICreationAudited;
            if (creationAudited == null)
            {
                return;
            }

            if (creationAudited.CreationTime == default)
            {
                creationAudited.CreationTime = DateTime.Now;
            }

            if (creationAudited.CreatorUserId != null)
            {
                return;
            }

            //Finally, set CreatorUserId!
            creationAudited.CreatorUserId = userId;
        }

        protected virtual void ApplyAbpConceptsForModifiedEntity(EntityEntry entry, string userId)
        {
            var modificationAudited = entry.Entity as IModificationAudited;
            if (modificationAudited == null)
            {
                return;
            }

            modificationAudited.LastModificationTime = DateTime.Now;
            modificationAudited.LastModifierUserId = userId;
        }

        protected virtual void SetDeletionAuditProperties(object entry, string userId)
        {
            var softDelete = entry as ISoftDelete;
            if (softDelete == null)
            {
                return;
            }

            softDelete.DeletionTime = DateTime.Now;
            softDelete.DeleterUserId = userId;
        }

        protected virtual void ApplyAbpConceptsForDeletedEntity(EntityEntry entry, string userId)
        {
            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity, userId);
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            //Set GUID Ids
            if (entry.Entity is IEntity<Guid> entity && entity.Id == Guid.Empty)
            {
                var idPropertyEntry = entry.Property("Id");

                if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
                {
                    entity.Id = CombGuid.NewGuid();
                }
            }
        }

        private string GetConnectionString(string config)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(config, optional: false);

            var configuration = builder.Build();
            return configuration.GetSection("IdentityServer4Admin").GetValue<string>("ConnectionString");
        }
    }
}