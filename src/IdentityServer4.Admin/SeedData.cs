using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Admin.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
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
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                {
                    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    context.Database.Migrate();
                    EnsureSeedData(context);
                }
                {
                    var context = scope.ServiceProvider.GetService<AdminDbContext>();
                    context.Database.Migrate();

                    if (!context.Users.Any())
                    {
                        AddRoles(scope).Wait();
                        AddUsers(scope).Wait();
                    }
                }
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }

        private static async Task AddUsers(IServiceScope scope)
        {
            await AddUser(scope, "admin", "1qazZAQ!", "zlzforever@163.com", "18721696556", "admin");
            await AddUser(scope, "songzhiyun", "1qazZAQ!", "songzhiyun@163.com", "18721696556", "expert-admin");
            await AddUser(scope, "shunyin", "1qazZAQ!", "shunyin@163.com", "18721696556", "expert-admin");
            await AddUser(scope, "dingjiaoyi", "1qazZAQ!", "dingjiaoyi@163.com", "18721696556", "expert-leader");
            await AddUser(scope, "yangjun", "1qazZAQ!", "yangjun@163.com", "18721696556", "expert-qc",
                "expert-op");
            await AddUser(scope, "wangliang", "1qazZAQ!", "wangliang@163.com", "18721696556", "expert");
            await AddUser(scope, "zousong", "1qazZAQ!", "zousong@163.com", "18721696556", "expert");
            await AddUser(scope, "shengwei", "1qazZAQ!", "shengwei@163.com", "18721696556", "expert");
        }

        private static async Task AddUser(IServiceScope scope, string name, string password, string email,
            string phone,
            params string[] roles)
        {
            //"18721696556"
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

        private static async Task AddRoles(IServiceScope scope)
        {
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var superAdmin = await roleMgr.FindByNameAsync("admin");
            if (superAdmin == null)
            {
                superAdmin = new Role
                {
                    Name = "admin",
                    NormalizedName = "Admin"
                };
                await roleMgr.CreateAsync(superAdmin);

                var expert = new Role
                {
                    Name = "expert",
                    NormalizedName = "Expert"
                };
                await roleMgr.CreateAsync(expert);

                var expertQc = new Role
                {
                    Name = "expert-qc",
                    NormalizedName = "Expert QC"
                };
                await roleMgr.CreateAsync(expertQc);

                var expertAdmin = new Role
                {
                    Name = "expert-admin",
                    NormalizedName = "Expert Admin"
                };
                await roleMgr.CreateAsync(expertAdmin);

                var expertOp = new Role
                {
                    Name = "expert-op",
                    NormalizedName = "Expert Operation"
                };
                await roleMgr.CreateAsync(expertOp);
                
                var expertLeader = new Role
                {
                    Name = "expert-leader",
                    NormalizedName = "Expert Leader"
                };
                await roleMgr.CreateAsync(expertLeader);
            }
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