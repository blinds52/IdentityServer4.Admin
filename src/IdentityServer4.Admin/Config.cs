using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer4.Admin
{
    internal class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var profile = new IdentityResources.Profile();
            profile.UserClaims.Add("role");
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                profile
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("expert-api", "Expert Api"),
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "expert-web",
                    ClientName = "Expert Web",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowedCorsOrigins = {"http://localhost:6568"},
                    RedirectUris = {"http://localhost:6568/account/ssocallback"},
                    PostLogoutRedirectUris = {"http://localhost:6568"},
                    RequireConsent = false,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "expert-api"
                    }
                }
            };
        }
    }
}