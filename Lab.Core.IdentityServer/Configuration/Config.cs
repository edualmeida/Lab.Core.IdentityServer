using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Lab.Core.IdentityServer.Configuration;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
                new ApiScope("userProfile", "Support API for the Identity services"),
                new ApiScope("api1", "api1 test"),
        };

    public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            ClientId = "identityApi",

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { "userProfile" }
        },
        // interactive ASP.NET Core MVC client
        new Client
        {
            ClientName = "mvc",
            ClientId = "mvc",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            // where to redirect to after login
            RedirectUris = { "https://localhost:5002/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
            AllowOfflineAccess = true,
            AlwaysSendClientClaims = true,
            AlwaysIncludeUserClaimsInIdToken = true,
            UpdateAccessTokenClaimsOnRefresh = true,
            
            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OfflineAccess,
                "api1"
            }
        },
        new Client
        {
            ClientId = "js",
            ClientName = "JavaScript Client",
            AllowedGrantTypes = GrantTypes.Code,
            RequireClientSecret = false,

            RedirectUris =           { "https://localhost:5003/callback.html" },
            PostLogoutRedirectUris = { "https://localhost:5003/index.html" },
            AllowedCorsOrigins =     { "https://localhost:5003" },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "api1"
            }
        }
    };
}
