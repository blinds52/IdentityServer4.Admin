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
                // new ApiResource("expert", "Expert System", new List<string> {"role"}),
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
                    ClientId = "expert",
                    ClientName = "Expert System",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RedirectUris = {"http://my.com:6568/signin-oidc"},
                    PostLogoutRedirectUris = {"http://my.com:6568/signout-callback-oidc"},

                    RequireConsent = false,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                }
            };
        }
    }
}