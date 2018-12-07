using System;
using System.Linq;
using System.Linq.Expressions;
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
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4.Admin
{
    public class AdminDbContext : IdentityDbContext<User, Role, Guid>, IDbContext,
        IDesignTimeDbContextFactory<AdminDbContext>, IConfigurationDbContext, IPersistedGrantDbContext
    {
        private readonly ConfigurationStoreOptions _configurationStoreOptions;
        private readonly OperationalStoreOptions _operationalStoreOptions;

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<UserPermission> UserPermissions { get; set; }

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureClientContext(_configurationStoreOptions);
            builder.ConfigureResourcesContext(_configurationStoreOptions);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions);

            builder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();
            builder.Entity<RolePermission>().HasIndex(p => new {p.RoleId, p.Permission}).IsUnique();
            builder.Entity<RolePermission>().HasIndex(p => new {p.RoleId, p.PermissionId}).IsUnique();
            builder.Entity<UserPermission>().HasIndex(p => new {p.UserId, p.Permission}).IsUnique();

            builder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserId);

            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            var userId = this.GetService<IHttpContextAccessor>()?.HttpContext?.User?.Identity.GetUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyAbpConcepts(entry, userId);
            }

            return base.SaveChanges();
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
                //Object does not implement IHasCreationTime
                return;
            }

            if (creationAudited.CreationTime == default(DateTime))
            {
                creationAudited.CreationTime = DateTime.Now;
            }

            if (creationAudited.CreatorUserId != null)
            {
                //CreatorUserId is already set
                return;
            }

            //Finally, set CreatorUserId!
            creationAudited.CreatorUserId = userId.ToString();
        }

        protected virtual void ApplyAbpConceptsForModifiedEntity(EntityEntry entry, string userId)
        {
            var modificationAudited = entry.Entity as IModificationAudited;
            if (modificationAudited == null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (modificationAudited.LastModificationTime == default(DateTime))
            {
                modificationAudited.LastModificationTime = DateTime.Now;
            }

            if (modificationAudited.LastModifierUserId != null)
            {
                //CreatorUserId is already set
                return;
            }

            //Finally, set CreatorUserId!
            modificationAudited.LastModifierUserId = userId;
        }

        protected virtual void SetDeletionAuditProperties(object entry, string userId)
        {
            var softDelete = entry as ISoftDelete;
            if (softDelete == null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (softDelete.DeletionTime == default(DateTime))
            {
                softDelete.DeletionTime = DateTime.Now;
            }

            if (softDelete.DeleterUserId != null)
            {
                //CreatorUserId is already set
                return;
            }

            //Finally, set CreatorUserId!
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

        public AdminDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AdminDbContext>();
            builder.UseSqlServer(GetConnectionString(args.Length > 0 ? args[0] : "appsettings.json"));

            return new AdminDbContext(builder.Options, new ConfigurationStoreOptions(), new OperationalStoreOptions());
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