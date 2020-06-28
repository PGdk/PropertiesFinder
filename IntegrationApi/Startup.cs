using System.Collections.Generic;
using DatabaseConnection;
using DatabaseConnection.Interfaces;
using DatabaseConnection.Repositories;
using Exhouse.Exhouse;
using Exhouse.Exhouse.Comparers;
using Exhouse.Interfaces;
using IntegrationApi.Interfaces;
using IntegrationApi.Security;
using IntegrationApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Utilities;

namespace IntegrationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDumpsRepository, DumpFileRepository>();
            services.AddSingleton<IEqualityComparer<Entry>, EntryComparer>();
            services.AddSingleton<IExhouseIntegration, ExhouseIntegration>();

            services.AddScoped<IEntriesRepository, EntriesRepository>();

            services.AddScoped<IEntriesAveragePricePerMeterCalculator, EntriesAveragePricePerMeterCalculator>();
            services.AddScoped<IEntryPointsCalculator, EntryPointsCalculator>();
            services.AddScoped<IOccasionalEntriesProvider, OccasionalEntriesProvider>();

            services.AddDbContext<DatabaseContext>();

            services.AddControllers();

            services
                .AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                        options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    }
                )
                .AddGoogle(
                    options =>
                    {
                        options.ClientId = "846045301625-ahquhcc0bu5qv2vm29df4a7bv0f4fb8j.apps.googleusercontent.com";
                        options.ClientSecret = "BL2Nn_z209fu7w-cRyPU59m4";
                    }
                )
                .AddCookie(
                    options =>
                    {
                        options.Cookie.HttpOnly = false;
                        options.Cookie.SameSite = SameSiteMode.None;
                        options.AccessDeniedPath = "/security/access_denied";
                    }
                );

            services.AddAuthorization(
                options => options.AddPolicy(
                    "User",
                    policy => policy.Requirements.Add(new UserPolicyRequirement())
                )
            );

            services.AddAuthorization(
                options => options.AddPolicy(
                    "Admin",
                    policy => policy.Requirements.Add(new AdminPolicyRequirement())
                )
            );

            services.AddSingleton<IAuthorizationHandler, UserPolicyHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminPolicyHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLogger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
