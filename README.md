# BlazorWebAssemblyTest
Blazor WebAssembly 3.2.0-preview2 example with rest api and identity auth services sharing models between the client and server. Utilises service / repository pattern.

Replaces AddApiAuthorization with AddOidcAuthentication to allow for custom clientid and scopes. Changes to IdentityServer4 startup.cs to accomodate this include replacing AddApiAuthorization with a more traditional setup of adding the clients, resources and identities in code, and replacing AddIdentityServerJwt with AddIdentityServerAuthentication to lock down api endpoints running on the same server.
