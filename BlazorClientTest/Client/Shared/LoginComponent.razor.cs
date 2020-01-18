using BlazorClientTest.Client.Auth;
using BlazorClientTest.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClientTest.Client.Shared
{
    public class LoginComponentBase : ComponentBase
    {
        protected LoginModel loginModel = new LoginModel();
        protected bool ShowErrors;
        protected string Error = "";

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public void HandleLogin()
        {
            ((CustomAuthenticationStateProvider)AuthenticationStateProvider).SetAuthenticated(true);
            NavigationManager.NavigateTo("/");
        }
    }
}