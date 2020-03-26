using BlazorWebAssemblyTest.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazorWebAssemblyTest.Shared.Interfaces;
using BlazorWebAssemblyTest.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlazorWebAssemblyTest.Server.Controllers
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
