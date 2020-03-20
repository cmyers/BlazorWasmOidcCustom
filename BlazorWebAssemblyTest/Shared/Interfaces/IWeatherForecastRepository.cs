using BlazorWebAssemblyTest.Shared.Models;
using System.Collections.Generic;

namespace BlazorWebAssemblyTest.Shared.Interfaces
{
    public interface IWeatherForecastRepository
    {
        IEnumerable<WeatherForecast> Get();
    }
}