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
            await AddUser(serviceProvider,
                Guid.NewGuid().ToString(),
                AdminConsts.AdminName,
                "admin", "admin",
                "1qazZAQ!", "zlzforever@163.com",
                "18721696556",
                "",
                "",
                "",
                "021-7998989",
                AdminConsts.AdminName);
        }

        private static async Task AddUsers(IServiceProvider serviceProvider)
        {
            var ids = new[]
            {
                "d237e995-572f-409c-8ca9-852e1d522ef0",
                "ee1d1d60-3a47-443e-8ef4-5412634aaca7",
                "994e9d44-821e-4b58-b207-185297cc7eab",
                "928844b3-4dbe-4d0a-9fee-7eac2d0369a6",
                "2bfd5be9-0caf-4f2d-9a80-e5d426749ce7",
                "8325387e-2ab4-42dd-9e3c-d98a0563a15f",
                "ae7a8445-4d8f-469c-823c-0c94ce4d0839",
                "d47b19e9-0cfb-4b95-bfde-97c48b91982e",
                "583cef3e-564c-4dbd-8fad-78d762fbad6a",
                "69a86e9f-2091-4fd6-84ac-90b9592cae80",
                "6b94a518-011d-414a-8330-1d3ce99a7ae7",
                "11ba7ad4-6230-44c0-a517-add90e0836a2",
                "11550f5f-fc5c-494e-b018-f03d1ad0ddfa",
                "98a4beb5-6ec0-4962-bb8d-e1a291ddb651",
                "a8e76fa1-89da-4c13-bf43-d6219c8e6d40",
                "d961ab17-8ad3-4a2b-a808-63529d67af7f",
                "6dc2c7e0-c51f-4d4b-b035-691cb5d7711f",
                "af70d04a-d79e-473f-b6d2-2193d594981e",
                "b150af67-59e5-4f77-861f-cd3f866cafbd",
                "7f8e17f7-03a0-4c09-997e-7ac5c9e3ba96",
                "5aca084c-7b3d-49c7-a694-62ff3784dd7e",
                "f47655b7-b77e-4522-ba3a-ed14b0cd3773",
                "02ccd0dd-1de8-4364-b74f-fe1e4b32fd7c",
                "137f199a-f5b5-4ec1-b05a-baaf4e92fa23",
                "b101c693-b5e1-4a6c-a5c1-1f8936f1ee58",
                "17c8bce3-a52c-41ad-883b-c2338de8d668",
                "0f571c50-3c74-4bce-add2-92d5e99aa79b",
                "02c4a35f-d0dd-48be-9f41-b7e3e379458b",
                "09b68a01-ccf3-4d80-b92d-ee193a2e3773",
                "b4d8a044-5c99-410f-a559-6e326f004d2b",
                "d1931d32-85b0-472e-a84b-c94068ee55bf",
                "cf693e0e-5ed6-4395-b341-ccb38085ff81",
                "5accc292-f42f-4bcf-a66c-eb6c14d8a313",
                "7dfb6851-6ae5-40e0-8061-ac94a090a0af",
                "40a08c9c-8d35-4588-8266-b8a7d707d3a3",
                "0161e23f-a3ca-4cd6-917d-322cb0964a80",
                "ca8497eb-fda7-45d5-99b4-e484d51df91f",
                "e5f0595c-2914-446c-b1c2-55101d6d10f6",
                "598a529c-2aab-4dda-9c63-a1b179dcc15d",
                "a1a361a7-7a14-4ad4-b6ca-04a5e3ea5929",
                "232cbaac-44cd-49fa-aacb-e4a2acd0647f",
                "0c472cb4-b25b-4b97-a82d-d6d410825455",
                "feafce75-ea20-4460-8a4b-73707024ed92",
                "119abebd-9380-4a2c-b20d-6326958c677f",
                "75fd1d9e-c311-4205-8091-2bb314d68479",
                "557783ef-d5f4-4249-86aa-ae45d789402d",
                "84a9f75c-1529-496e-bcd2-bd0975b7a1d5",
                "2963924a-3a2b-419b-8b46-b33e4a8f6221",
                "ec607691-dbba-4cfd-b0b6-d0b52fdaedc7",
                "e1ce6a2c-79e7-4c6c-98fe-b9da1c9bd3bd",
                "e42db022-9d39-48dd-8784-9834cf39b035",
                "217dc07f-a1ea-44e1-903b-28f59ad05c5c",
                "d6c7e572-7500-41d6-8e99-da56f3a48bb0",
                "77753d74-f412-46b9-8428-7d83794c4681",
                "73fdbc51-ef00-4593-befd-82567766ee8e",
                "d9e44d7a-153e-4903-9ef7-d8116ff776b8",
                "ad229877-778b-4fb4-986f-f0e00591fbf3",
                "bc55b7e2-a71d-49cc-86a7-d489e5e6b321",
                "3100da87-8905-421a-90d5-3e7a93d8df46",
                "c5aaf8b1-571f-454c-994a-2e07ce75fc2b",
                "aa3579cb-7b32-4886-9931-7ee69e67d07d",
                "3c8339c7-def5-4c19-bcd9-22d60506fc64",
                "5b12ebe8-d898-44aa-b276-4898f985aa40",
                "ea6dc37b-64da-4723-bdb8-c9205475151c",
                "4b0b4d04-4360-4109-9fcc-e280ed7fe1a8",
                "ce3e828f-c1a3-45e5-a87b-8daf4c75bf94",
                "9ff836de-9806-4889-b63d-c6e72829a696",
                "e8734e6d-56dc-4d82-9839-fca815eb2329",
                "54dec57a-e990-4b38-bbef-be63f5bd3c54",
                "18f69c8f-6c22-486b-85ab-ee999cc17312"
            };
            for (int i = 0; i < 30; ++i)
            {
                await AddUser(serviceProvider,
                    ids[i],
                    "User" + i,
                    "FirstName " + i,
                    "LastName " + i,
                    "1qazZAQ!",
                    "email" + i + "@163.com",
                    "13899981" + i,
                    "专家团队", "A2", "咨询师", "021-7896651" + i,
                    "expert");
            }

            for (int i = 30; i < 60; ++i)
            {
                await AddUser(serviceProvider,
                    ids[i],
                    "User" + i,
                    "FirstName " + i,
                    "LastName " + i,
                    "1qazZAQ!",
                    "email" + i + "@163.com",
                    "13899981" + i,
                    "销售团队", "A2", "机构销售", "021-7896651" + i,
                    "sale");
            }
        }

        private static async Task AddUser(IServiceProvider serviceProvider, string id, string name, string firstName,
            string lastName, string password, string email,
            string phone, string group, string level, string title, string officePhone,
            params string[] roles)
        {
            var userMgr = serviceProvider.GetRequiredService<UserManager<User>>();
            var user = new User
            {
                Id = Guid.Parse(id),
                UserName = name,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Group = group,
                EmailConfirmed = true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = true,
                Level = level,
                Title = title,
                OfficePhone = officePhone,
                TwoFactorEnabled = true,
                Sex = 0,
                LockoutEnabled = true,
                CreationTime = DateTime.Now,
                IsDeleted = false,
                AccessFailedCount = 0
            };
            var result = await userMgr.CreateAsync(user, password);
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
            await AddRole(serviceProvider, "expert", "A member of expert group");
            await AddRole(serviceProvider, "expert-qc", "The quality controller of expert team");
            await AddRole(serviceProvider, "expert-admin", "The admin of expert group");
            await AddRole(serviceProvider, "expert-op", "The operator of expert group");
            await AddRole(serviceProvider, "expert-leader", "The leader of a expert team");
            await AddRole(serviceProvider, "sale", "销售");
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