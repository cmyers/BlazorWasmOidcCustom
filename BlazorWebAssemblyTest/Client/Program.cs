using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorWebAssemblyTest.Shared.Interfaces;
using BlazorWebAssemblyTest.Shared.Services;

namespace BlazorWebAssemblyTest.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddApiAuthorization();
            builder.Services.AddSingleton<ICounterService, CounterService>();

            await builder.Build().RunAsync();
        }
    }
}
