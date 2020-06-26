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

namespace BlazorWasmOidcCustom.Server
{
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
               .AddInMemoryApiResources(new ApiResourceCollection
                    {
                        new ApiResource
                        {
                            Name = "Weather.Aud",
                            DisplayName = "Weather API",
                            Description = "Weather API Test",
                            ApiSecrets = new List<Secret>
                            {
                                new Secret("{somesecret}".Sha256())
                            },
                            //User claims are added to the access token's requested claim types automatically and then accessed in the profile service.
                            UserClaims = new List<string> {JwtClaimTypes.Role},
                            //Scopes define what the resource can do
                            Scopes =
                            {
                                new Scope()
                                {
                                    Name = "Weather.Read",
                                    DisplayName = "Weather Read",
                                    UserClaims = { "Weather.Access.Read" }
                                },
                                new Scope()
                                {
                                    Name = "Weather.Write",
                                    DisplayName = "Weather Write",
                                    UserClaims = { "Weather.Access.Write" }
                                },
                            }
                        }
                    })
               .AddInMemoryClients(new ClientCollection
                    {
                        new IdentityServer4.Models.Client
                        {
                            ClientId = "BlazorClient",
                            ClientName = "Weather.Aud",
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
                            RedirectUris = {"https://localhost:44303/authentication/login-callback"},
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
                options.AddPolicy("WeatherPolicy.Write", builder => {
                    builder.RequireRole("admin");
                    builder.RequireScope("Weather.Write");
                });
                options.AddPolicy("WeatherPolicy.Read", builder => {
                    builder.RequireRole(new List<string> { "user", "admin" });
                    builder.RequireScope("Weather.Read");
                });
            });

            services.AddAuthentication()
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:44303";
                    options.RequireHttpsMetadata = true;
                    options.ApiName = "Weather.Aud";
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
                app.UseDatabaseErrorPage();
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "areas", 
                    areaName: "identity",
                    pattern: "identity/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}


