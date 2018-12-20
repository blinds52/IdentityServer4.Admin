using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiResource = IdentityServer4.Models.ApiResource;
using Client = IdentityServer4.Models.Client;
using GrantTypes = IdentityServer4.Models.GrantTypes;
using IdentityResource = IdentityServer4.Models.IdentityResource;


namespace IdentityServer4.Admin
{
    internal static class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding database...");

            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // Add client
                await AddClients(scope.ServiceProvider);
                // Add identityResource
                await AddIdentityResources(scope.ServiceProvider);
                // Add apiResource
                await AddApiResources(scope.ServiceProvider);

                await scope.ServiceProvider.GetRequiredService<IDbContext>().SaveChangesAsync();

                var dbContext = (AdminDbContext) scope.ServiceProvider.GetRequiredService<IDbContext>();
                if (await dbContext.Users.CountAsync() == 0)
                {
                    await AddPermissions(scope.ServiceProvider);
                    await AddAdminRole(scope.ServiceProvider);
                    await AddRoles(scope.ServiceProvider);
                    await AddAdmin(scope.ServiceProvider);
                    await AddUsers(scope.ServiceProvider);
                }

                await CommitAsync(serviceProvider);
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }

        private static async Task AddAdminRole(IServiceProvider serviceProvider)
        {
            var context = (AdminDbContext) serviceProvider.GetRequiredService<IDbContext>();

            var role = await AddRole(serviceProvider, AdminConsts.AdminName, "Super admin");
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == AdminConsts.AdminName);
            await context.RolePermissions.AddAsync(new RolePermission
                {RoleId = role.Id, PermissionId = permission.Id});
            await CommitAsync(serviceProvider);
        }

        private static async Task AddPermissions(IServiceProvider serviceProvider)
        {
            var context = (AdminDbContext) serviceProvider.GetRequiredService<IDbContext>();

            var permission = new Permission
                {Name = AdminConsts.AdminName, Description = "Super admin permission"};
            await context.Permissions.AddAsync(permission);
            await CommitAsync(serviceProvider);
        }

        private static async Task CommitAsync(IServiceProvider serviceProvider)
        {
            await serviceProvider.GetRequiredService<IDbContext>().SaveChangesAsync();
        }

        private static async Task AddApiResources(IServiceProvider serviceProvider)
        {
            var context = (AdminDbContext) serviceProvider.GetRequiredService<IDbContext>();
            if (!await context.ApiResources.AnyAsync())
            {
                Console.WriteLine("ApiResources being populated");
                foreach (var resource in GetApiResources().ToList())
                {
                    await context.ApiResources.AddAsync(resource.ToEntity());
                }
            }
            else
            {
                Console.WriteLine("ApiResources already populated");
            }
        }

        private static async Task AddIdentityResources(IServiceProvider serviceProvider)
        {
            var context = (AdminDbContext) serviceProvider.GetRequiredService<IDbContext>();
            if (!await context.IdentityResources.AnyAsync())
            {
                Console.WriteLine("IdentityResources being populated");

                foreach (var resource in GetIdentityResources().ToList())
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }
            }
            else
            {
                Console.WriteLine("IdentityResources already populated");
            }
        }

        private static async Task AddClients(IServiceProvider serviceProvider)
        {
            var context = (AdminDbContext) serviceProvider.GetRequiredService<IDbContext>();
            if (!await context.Clients.AnyAsync())
            {
                Console.WriteLine("Clients being populated");
                foreach (var client in GetClients().ToList())
                {
                    await context.Clients.AddAsync(client.ToEntity());
                }
            }
            else
            {
                Console.WriteLine("Clients already populated");
            }
        }

        private static async Task AddAdmin(IServiceProvider serviceProvider)
        {
            await AddUser(serviceProvider, AdminConsts.AdminName, "1qazZAQ!", "zlzforever@163.com", "18721696556",
                AdminConsts.AdminName);
        }

        private static async Task AddUsers(IServiceProvider serviceProvider)
        {
            await AddUser(serviceProvider, "songzhiyun", "1qazZAQ!", "songzhiyun@163.com", "18121696556", "expert",
                "expert-admin");
            await AddUser(serviceProvider, "shunyin", "1qazZAQ!", "shunyin@163.com", "18221696556", "expert",
                "expert-admin", "UserReader");
            await AddUser(serviceProvider, "dingjiaoyi", "1qazZAQ!", "dingjiaoyi@163.com", "18321696556", "expert",
                "expert-leader");
            await AddUser(serviceProvider, "yangjun", "1qazZAQ!", "yangjun@163.com", "18421696556", "expert",
                "expert-qc",
                "expert-op");
            await AddUser(serviceProvider, "wangliang", "1qazZAQ!", "wangliang@163.com", "18521696556", "expert");
            await AddUser(serviceProvider, "zousong", "1qazZAQ!", "zousong@163.com", "18621696556", "expert");
            await AddUser(serviceProvider, "shengwei", "1qazZAQ!", "shengwei@163.com", "18721696556", "expert");
        }

        private static async Task AddUser(IServiceProvider serviceProvider, string name, string password, string email,
            string phone,
            params string[] roles)
        {
            var userMgr = serviceProvider.GetRequiredService<UserManager<User>>();
            var user = new User
            {
                UserName = name,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = true
            };
            await userMgr.CreateAsync(user, password);
            await userMgr.AddClaimsAsync(user, new[]
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.Email, email),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
            });
            foreach (var role in roles)
            {
                await userMgr.AddToRoleAsync(user, role);
            }
        }

        private static async Task<Role> AddRole(IServiceProvider serviceProvider, string name, string description)
        {
            var roleMgr = serviceProvider.GetRequiredService<RoleManager<Role>>();

            var role = await roleMgr.FindByNameAsync(name);
            if (role == null)
            {
                role = new Role
                {
                    Name = name,
                    Description = description
                };
                var result = await roleMgr.CreateAsync(role);
                return result.Succeeded ? role : null;
            }

            return null;
        }

        private static async Task AddRoles(IServiceProvider serviceProvider)
        {
            await AddRole(serviceProvider, "UserReader", "Which person can read all users");
            await AddRole(serviceProvider, "expert", "A member of expert group");
            await AddRole(serviceProvider, "expert-qc", "The quality controller of expert team");
            await AddRole(serviceProvider, "expert-admin", "The admin of expert group");
            await AddRole(serviceProvider, "expert-op", "The operator of expert group");
            await AddRole(serviceProvider, "expert-leader", "The leader of a expert team");
            await CommitAsync(serviceProvider);
        }

        private static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("expert-api", "专家团队模块", new List<string> {JwtClaimTypes.Role}),
                new ApiResource("identity-server4", "IdentityServer4", new List<string> {JwtClaimTypes.Role})
            };
        }

        // scopes define the resources in your system
        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var openId = new IdentityResources.OpenId();
            openId.DisplayName = "用户标识";

            var profile = new IdentityResources.Profile();
            profile.DisplayName = "资料: 如姓、名、角色等";
            profile.Description = "";
            profile.UserClaims.Add(JwtClaimTypes.Role);

            return new List<IdentityResource>
            {
                openId,
                profile
            };
        }

        // clients want to access resources (aka scopes)
        private static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "vue-expert",
                    ClientName = "vue-expert",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowedCorsOrigins = {"http://localhost:6568"},
                    RedirectUris = {"http://localhost:6568/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:6568/signout-callback-oidc"},
                    RequireConsent = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "expert-api",
                        "identity-server4"
                    }
                },
                new Client
                {
                    ClientId = "email-proxy",
                    ClientName = "Email Proxy",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedCorsOrigins = {"http://localhost:6570"},
                    RedirectUris = {"http://localhost:6570/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:6570/signout-callback-oidc"},
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    RequireConsent = false
                }
            };
        }
    }
}