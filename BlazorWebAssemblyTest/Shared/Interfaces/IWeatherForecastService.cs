using BlazorWebAssemblyTest.Shared.Models;
using System.Collections.Generic;

namespace BlazorWebAssemblyTest.Shared.Interfaces
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}
