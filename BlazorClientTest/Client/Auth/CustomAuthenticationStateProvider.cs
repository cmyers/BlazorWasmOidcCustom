using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorClientTest.Client.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            bool authenticated = true;
            ClaimsIdentity identity;

            if (authenticated)
            {
                identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "TestUser")

                }, "WebApiAuth");
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        }

    }
}
