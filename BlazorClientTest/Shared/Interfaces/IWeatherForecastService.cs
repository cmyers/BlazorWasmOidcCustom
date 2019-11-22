using BlazorClientTest.Shared.Models;
using System.Collections.Generic;

namespace BlazorClientTest.Shared.Interfaces
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}
