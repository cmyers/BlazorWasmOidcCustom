using BlazorClientTest.Shared.Models;
using System.Collections.Generic;

namespace BlazorClientTest.Shared.Interfaces
{
    public interface IWeatherForecastRepository
    {
        IEnumerable<WeatherForecast> Get();
    }
}