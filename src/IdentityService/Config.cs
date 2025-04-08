using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction app full access"),
            new ApiScope("scope2"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "postman",
                ClientName = "postman",
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                ClientSecrets = { new Secret("Secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            },
            new Client
            {
                ClientId = "nextApp",
                ClientName = "nextApp",
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "http://localhost:3000/api/auth/callback/id-server" },
                ClientSecrets = { new Secret("Secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                // This is false because the app is not native, in a react appliction this would be true and AllowedGrantTypes whould be code
                RequirePkce = false,
                AllowOfflineAccess = true
            },
        };
}
