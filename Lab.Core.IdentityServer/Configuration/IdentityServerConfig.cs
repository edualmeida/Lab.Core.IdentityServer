using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Lab.Core.IdentityServer.Configuration;

public static class IdentityServerConfig
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
                new ApiScope(AppConstants.IdentityResource_AccountManager, "Support API for the identity services account controller"),
                new ApiScope("api1", "api test"),
        };

    public static IEnumerable<Client> Clients (IConfiguration configuration) =>
    new List<Client>
    {
        new Client
        {
            ClientId = AppConstants.Client_ClientId_IdentityApi,

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
            {
                new Secret("DJ%?s$,@S!xEQmt7V_p&6Z".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { AppConstants.IdentityResource_AccountManager }
        },
        // interactive ASP.NET Core MVC client
        new Client
        {
            ClientName = AppConstants.Client_ClientId_LabGymWeb,
            ClientId = AppConstants.Client_ClientId_LabGymWeb,
            ClientSecrets = { new Secret("J2-6zNR/pLf5k>wAK&.B%$".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
            
            RedirectUris = { configuration["Ids.Clients:LabGymWeb:RedirectUris"] }, // where to redirect to after login

            PostLogoutRedirectUris = { configuration["Ids.Clients:LabGymWeb:PostLogoutRedirectUris"] }, // where to redirect to after logout
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
