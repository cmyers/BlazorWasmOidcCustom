using BlazorClientTest.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorClientTest.Client.Pages
{
    public class WeatherForecastPageBase : ComponentBase
    {
        protected WeatherForecast[] forecasts;

        [Inject]
        public HttpClient Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            forecasts = await Http.GetJsonAsync<WeatherForecast[]>("api/WeatherForecast");
        }
    }
}
