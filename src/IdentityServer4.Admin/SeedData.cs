using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Admin
{
    internal static class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding database...");

            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var hostEnv = scope.ServiceProvider.GetService<IHostingEnvironment>();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                if (!hostEnv.IsDevelopment())
                {
                    scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                    configurationDbContext.Database.Migrate();
                }

                EnsureSeedData(configurationDbContext);

                var context = scope.ServiceProvider.GetService<AdminDbContext>();
                if (!hostEnv.IsDevelopment())
                {
                    context.Database.Migrate();
                }

                if (!context.Users.Any())
                {
                    AddAdmin(scope).ConfigureAwait(true);
                    AddRoles(scope).ConfigureAwait(true);
                    AddUsers(scope).ConfigureAwait(true);
                }
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }

        private static async Task AddAdmin(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<AdminDbContext>();
            var permission = new Permission
                {Name = AdminConsts.AdminName, Description = "Super admin permission"};
            await dbContext.Permissions.AddAsync(permission);
            var role = await AddRole(scope, AdminConsts.AdminName, "Super admin");
            await dbContext.RolePermissions.AddAsync(new RolePermission
                {Permission = AdminConsts.AdminName, RoleId = role.Id, PermissionId = permission.Id});
            await AddUser(scope, AdminConsts.AdminName, "1qazZAQ!", "zlzforever@163.com", "18721696556",
                AdminConsts.AdminName);
        }

        private static async Task AddUsers(IServiceScope scope)
        {
            await AddUser(scope, "songzhiyun", "1qazZAQ!", "songzhiyun@163.com", "18721696556", "expert-admin");
            await AddUser(scope, "shunyin", "1qazZAQ!", "shunyin@163.com", "18721696556", "expert-admin");
            await AddUser(scope, "dingjiaoyi", "1qazZAQ!", "dingjiaoyi@163.com", "18721696556", "expert-leader");
            await AddUser(scope, "yangjun", "1qazZAQ!", "yangjun@163.com", "18721696556", "expert-qc", "expert-op");
            await AddUser(scope, "wangliang", "1qazZAQ!", "wangliang@163.com", "18721696556", "expert");
            await AddUser(scope, "zousong", "1qazZAQ!", "zousong@163.com", "18721696556", "expert");
            await AddUser(scope, "shengwei", "1qazZAQ!", "shengwei@163.com", "18721696556", "expert");
        }

        private static async Task AddUser(IServiceScope scope, string name, string password, string email,
            string phone,
            params string[] roles)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var admin = new User
            {
                UserName = name,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = true
            };
            await userMgr.CreateAsync(admin, password);
            await userMgr.AddClaimsAsync(admin, new[]
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.Email, email),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
            });
            foreach (var role in roles)
            {
                await userMgr.AddToRoleAsync(admin, role);
            }
        }

        private static async Task<Role> AddRole(IServiceScope scope, string name, string description)
        {
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var role = await roleMgr.FindByNameAsync(name);
            if (role == null)
            {
                role = new Role
                {
                    Name = name,
                    Description = description
                };
                await roleMgr.CreateAsync(role);
                return role;
            }

            return null;
        }

        private static async Task AddRoles(IServiceScope scope)
        {
            await AddRole(scope, "expert", "A member of expert group");
            await AddRole(scope, "expert-qc", "The quality controller of expert team");
            await AddRole(scope, "expert-admin", "The admin of expert group");
            await AddRole(scope, "expert-op", "The operator of expert group");
            await AddRole(scope, "expert-leader", "The leader of a expert team");
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients being populated");
                foreach (var client in Config.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources being populated");
                foreach (var resource in Config.GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("IdentityResources already populated");
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources being populated");
                foreach (var resource in Config.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("ApiResources already populated");
            }
        }
    }
}