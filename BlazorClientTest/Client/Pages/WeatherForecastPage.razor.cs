using BlazorClientTest.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClientTest.Client.Pages
{
    [Authorize]
    public class WeatherForecastPageBase : ComponentBase
    {
        protected WeatherForecast[] forecasts;

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IAccessTokenProvider AuthenticationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Navigation.BaseUri);

            var tokenResult = await AuthenticationService.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Value}");
                forecasts = await httpClient.GetJsonAsync<WeatherForecast[]>("api/WeatherForecast");
            }
            else
            {
                Navigation.NavigateTo(tokenResult.RedirectUrl);
            }

        }
    }
}
