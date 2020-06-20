using BlazorWasmOidcCustom.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazorWasmOidcCustom.Shared.Interfaces;
using BlazorWasmOidcCustom.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlazorWasmOidcCustom.Server.Controllers
{
    [Authorize(Policy = "Resource.API.Test", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation(Request.Path.ToString());
            return _weatherForecastService.Get();
        }
    }
}
