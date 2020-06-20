# BlazorWasmOidcCustom
Blazor WebAssembly 3.2.0 example with rest api and identity auth server sharing models between the client and server. Utilises service / repository pattern.

Replaces AddApiAuthorization with AddOidcAuthentication to allow for custom clientid and scopes. Changes to IdentityServer4 startup.cs to accommodate this include replacing AddApiAuthorization with adding the clients, resources and identities in code for testing, and replacing AddIdentityServerJwt with AddIdentityServerAuthentication to lock down api endpoints running on the same server.
