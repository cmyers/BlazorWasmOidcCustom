using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorClientTest.Client.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private bool _authenticated = false;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity;

            if (_authenticated)
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

        public void SetAuthenticated (bool authenticated)
        {
            _authenticated = authenticated;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

    }
}
