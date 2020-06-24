using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DatabaseConnection;
using IntegrationApi;
using Models;
using Utilities;
using Microsoft.AspNetCore.Http;
using IntegrationApi.Polices;
using IntegrationSprzedajemy;
using IntegrationSprzedajemyService;

namespace IntegrationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ISprzedajemyRepository), new SprzedajemyRepository());
            services.AddSingleton(typeof(IDumpsRepository), new DumpFileRepository());
            services.AddSingleton(typeof(IEqualityComparer<Entry>), new Comparer());
            services.AddSingleton(typeof(ISprzedajemyService), new SprzedajemyService(new SprzedajemyRepository()));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGoogle(options =>
            {
                options.ClientId = "211669332408-5v5f0lfe924ccriqgibsppm707gs7pmt.apps.googleusercontent.com";
                options.ClientSecret = "NiLLHIWUiPKmddwGzExbGfNm";
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            services.AddControllers();

            services.AddAuthorization(
                options => options.AddPolicy("User",
                    policy => policy.Requirements.Add(new UserPolicyRequirement())));
            services.AddAuthorization(
                options => options.AddPolicy("Admin",
                    policy => policy.Requirements.Add(new AdminPolicyRequirement())));

            services.AddSingleton<IAuthorizationHandler, UserPolicy>();
            services.AddSingleton<IAuthorizationHandler, AdminPolicy>();
            services.AddMvc();
        }
        private Task XRequestIDMiddleware(HttpContext context)
        {
            return Task.Run(() =>
            {
                var request = context.Request;
                var databaseRepository = context.RequestServices.GetService<ISprzedajemyRepository>();

                if (request.Headers.ContainsKey("X-Request-ID"))
                {
                    if (request.Headers["X-Request-ID"].ToString().Trim() != "")
                    {
                        databaseRepository.AddLog(request.Headers["X-Request-ID"]);
                    }
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await XRequestIDMiddleware(context);
                await next();
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}