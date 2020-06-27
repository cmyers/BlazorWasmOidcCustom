using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorWasmOidcCustom.Shared.Interfaces;
using BlazorWasmOidcCustom.Shared.Services;

namespace BlazorWasmOidcCustom.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = "https://localhost:44303";
                options.ProviderOptions.ClientId = "BlazorClient";
                options.ProviderOptions.ResponseType = "code";
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("email");
                options.ProviderOptions.DefaultScopes.Add("offline_access");
                options.ProviderOptions.DefaultScopes.Add("Weather.Read");
                options.ProviderOptions.DefaultScopes.Add("Weather.Write");
                options.AuthenticationPaths.RemoteRegisterPath = "Identity/Account/Register";
                options.AuthenticationPaths.RemoteProfilePath = "Identity/Account/Manage";
            });
            builder.Services.AddSingleton<ICounterService, CounterService>();

            await builder.Build().RunAsync();
        }
    }
}
