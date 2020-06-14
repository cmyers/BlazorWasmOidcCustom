using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorWebAssemblyTest.Server.Data;
using BlazorWebAssemblyTest.Server.Models;
using BlazorWebAssemblyTest.Shared.Interfaces;
using BlazorWebAssemblyTest.Shared.Services;
using BlazorWebAssemblyTest.Shared.Repositories;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.AccessTokenValidation;
using BlazorWebAssemblyTest.Server.Services;

namespace BlazorWebAssemblyTest.Server
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
               .AddInMemoryApiResources(new ApiResourceCollection
                    {
                        new ApiResource
                        {
                            Name = "Resource.API.Test",
                            ApiSecrets = new List<Secret>
                            {
                                new Secret("{somesecret}".Sha256())
                            },
                            Scopes =
                            {
                                new Scope()
                                {
                                    Name = "Resource.API.Test.access",
                                    DisplayName = "Full access",
                                    UserClaims = {"Resource.API.Test.access.level" }
                                }
                            }
                        }
                    })
               .AddInMemoryClients(new ClientCollection
                    {
                        new IdentityServer4.Models.Client
                        {
                            ClientId = "blazorclient",
                            ClientName = "Resource.API.Test",
                            AllowedGrantTypes = GrantTypes.Code,
                            RequirePkce = true,
                            AllowedScopes = new List<string> {
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile,
                                IdentityServerConstants.StandardScopes.Email,
                                IdentityServerConstants.StandardScopes.OfflineAccess,
                                "Resource.API.Test.access"},
                            AllowOfflineAccess = true,
                            RedirectUris = {"https://localhost:44303/authentication/login-callback"},
                            PostLogoutRedirectUris = { "https://localhost:44303/authentication/logout-callback" },
                            RequireClientSecret = false,
                            RequireConsent = false
                        }
                    })
               .AddAspNetIdentity<ApplicationUser>()
               .AddProfileService<AuthProfileService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Resource.API.Test", builder => builder.RequireClaim("access_level", "auth.admin"));
            });

            services.AddAuthentication()
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:44303";
                    options.RequireHttpsMetadata = true;
                    options.ApiName = "Resource.API.Test";
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
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
