using BlazorWasmOidcCustom.Shared.Models;
using System.Collections.Generic;

namespace BlazorWasmOidcCustom.Shared.Interfaces
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}
