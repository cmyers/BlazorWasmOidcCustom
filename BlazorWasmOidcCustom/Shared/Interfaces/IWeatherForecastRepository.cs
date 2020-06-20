using BlazorWasmOidcCustom.Shared.Models;
using System.Collections.Generic;

namespace BlazorWasmOidcCustom.Shared.Interfaces
{
    public interface IWeatherForecastRepository
    {
        IEnumerable<WeatherForecast> Get();
    }
}