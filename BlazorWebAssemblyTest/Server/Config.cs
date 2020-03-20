using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace BlazorWebAssemblyTest.Server
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        //Protected APIs
        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "API.Rest",
                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "rest.access",
                            DisplayName = "Basic access rights",
                            UserClaims = new [] { "rest.access.level" }
                        },
                    }
                },
                new ApiResource
                {
                    Name = "API.Test1",
                    Scopes = { 
                        new Scope()
                        {
                            Name = "test1.access",
                            DisplayName = "Test1 Details",
                            //these are the requested claim types that get sent to the profile service if we request lastfm_info. We can then inject claims if we want to send back here
                            UserClaims = new [] { "test1.info" }
                        },
                    }
                },
                new ApiResource
                {
                    Name = "API.Auth",
                    ApiSecrets = new List<Secret>
                    {
                        new Secret("220a4266-7e92-4890-9774-cc8c79893adc".Sha256())
                    },
                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "auth.access",
                            DisplayName = "Full access",
                            UserClaims = new [] { "auth.access.level" }
                        }
                    }
                }
            };

        //Clients that are allowed to request tokens:
        public static IEnumerable<IdentityServer4.Models.Client> Clients =>
            new List<IdentityServer4.Models.Client>
            {
                new IdentityServer4.Models.Client
                {
                    ClientId = "f3477aaa-5f58-4b07-a576-133ff98617d8",
                    ClientName = "RestAccessClientDev",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("9e2a98f2-a7d2-478e-b488-062f559088b3".Sha256())
                    },
                    AllowedScopes = 
                    { 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email, 
                        "rest.access", 
                        "auth.access",
                        "lastfm.access"
                    },
                    AccessTokenLifetime = 30,
                    AllowOfflineAccess = true,
                },
                new IdentityServer4.Models.Client
                {
                    ClientId = "f3477aac-5f58-4b07-a576-133ff98617d8",
                    ClientSecrets =
                    {
                        new Secret("8d826deb-9d96-4e4a-89c1-5ea109ff0a16".Sha256())
                    },
                    ClientName = "AuthAccessClientCode",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes = new List<string> {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email, 
                        "rest.access", 
                        "test1.access"},
                    RedirectUris = new List<string> {"http://localhost:3000/signin-callback"},
                    PostLogoutRedirectUris = new List<string> {"http://localhost:3000/signout-callback"},
                    AllowedCorsOrigins = new List<string> {"http://localhost:3000"},
                    AllowOfflineAccess = true
                }
                //https://www.scottbrady91.com/Angular/Migrating-oidc-client-js-to-use-the-OpenID-Connect-Authorization-Code-Flow-and-PKCE
            };
    }
}