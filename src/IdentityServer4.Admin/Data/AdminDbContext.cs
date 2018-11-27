using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Data
{
    public class AdminDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<UserPermission> UserPermissions { get; set; }

        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();
            builder.Entity<RolePermission>().HasIndex(p => new {p.RoleId, p.Permission}).IsUnique();
            builder.Entity<RolePermission>().HasIndex(p => new {p.RoleId, p.PermissionId}).IsUnique();
            builder.Entity<UserPermission>().HasIndex(p => new {p.UserId, p.Permission}).IsUnique();
            builder.Entity<UserPermission>().HasIndex(p => new {p.UserId, p.PermissionId}).IsUnique();
        }
    }
}