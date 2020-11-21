using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorWasmOidcCustom.Server.Data;
using BlazorWasmOidcCustom.Server.Models;
using BlazorWasmOidcCustom.Shared.Interfaces;
using BlazorWasmOidcCustom.Shared.Services;
using BlazorWasmOidcCustom.Shared.Repositories;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.AccessTokenValidation;
using BlazorWasmOidcCustom.Server.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWasmOidcCustom.Server
{

    public static class AuthorizationPolicies
    {
        public const string WeatherRead = "WeatherPolicy.Read";
        public const string WeatherWrite = "WeatherPolicy.Write";
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
               .AddDeveloperSigningCredential()
               .AddInMemoryPersistedGrants()
               .AddInMemoryIdentityResources(new IdentityResourceCollection
                    {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        new IdentityResources.Email()
                    })
               .AddInMemoryApiScopes(new ApiScopeCollection
               {
                   new ApiScope
                   {
                       Name = "Weather.Read",
                       DisplayName = "Weather Read",
                       UserClaims = { "Weather.Access.Read" }
                   },
                   new ApiScope
                   {
                       Name = "Weather.Write",
                       DisplayName = "Weather Write",
                       UserClaims = { "Weather.Access.Write" }
                   }
               })
               .AddInMemoryApiResources(new ApiResourceCollection
                    {
                        new ApiResource
                        {
                            Name = "Weather.Aud", //This value identifies the audience
                            DisplayName = "Weather API",
                            Description = "Weather API Test",
                            ApiSecrets = new List<Secret>
                            {
                                new Secret("{somesecret}".Sha256())
                            },
                            //UserClaims are added to the access token's requested claim types automatically and then accessed in the profile service.
                            UserClaims = new List<string> {JwtClaimTypes.Role},
                            //Scopes define what the resource can do
                            Scopes =
                            {
                                "Weather.Read",
                                "Weather.Write",
                            }
                        }
                    })
               .AddInMemoryClients(new ClientCollection
                    {
                        new IdentityServer4.Models.Client
                        {
                            ClientId = "BlazorClient",
                            ClientName = "WeatherClient",
                            AllowedGrantTypes = GrantTypes.Code,
                            RequirePkce = true,
                            AllowedScopes = new List<string> {
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile,
                                IdentityServerConstants.StandardScopes.Email,
                                IdentityServerConstants.StandardScopes.OfflineAccess,
                                "Weather.Read",
                                "Weather.Write"},
                            AllowOfflineAccess = true,
                            RedirectUris = {"https://localhost:44303/authentication/login-callback", "https://oauth.pstmn.io/v1/callback"},
                            PostLogoutRedirectUris = { "https://localhost:44303/authentication/logout-callback" },
                            RequireClientSecret = false,
                            RequireConsent = false
                        }
                    })
               .AddAspNetIdentity<ApplicationUser>()
               .AddProfileService<AuthProfileService>();

            /* In order to make use of scopes and role claims together the policies below are required.
             * The appropriate policy is then registered in the Authorize attribute against the controller or method that requires access locking down.
             * This would mean if a client didn't send its intention to use a certain scope then the policy would be void.
             * This also means if the user didn't have the correct role claim to allow the client to use the correct scope then this would also void the policy.
             */
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.WeatherWrite, builder => {
                    builder.RequireRole("admin");
                    builder.RequireScope("Weather.Write");
                });
                options.AddPolicy(AuthorizationPolicies.WeatherRead, builder => {
                    builder.RequireRole(new List<string> { "user", "admin" });
                    builder.RequireScope("Weather.Read");
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:44303";
                    options.RequireHttpsMetadata = true;
                    options.Audience = "Weather.Aud";
                });

            services.AddTransient<IWeatherForecastService, WeatherForecastService>();
            services.AddTransient<IWeatherForecastRepository, WeatherForecastRepository>();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}


