using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseConnection;
using GazetaKrakowska;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IGazetaKrakowskaRepository>(s => new GazetaKrakowskaRepository(new GazetaKrakowskaContext()));
            services.AddSingleton(typeof(IDumpsRepository), new DumpFileRepository());
            services.AddSingleton(typeof(IEqualityComparer<Entry>), new GazetaKrakowskaComparer());

            services.AddAutoMapper(typeof(Startup));
            services.AddAutoMapper(c => c.AddProfile<AutoMapping>(), typeof(Startup));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGoogle(options =>
            {
                options.ClientId = "890033233825-764v59iiaggk7i13h0ho3e1pq0b4016l.apps.googleusercontent.com";
                options.ClientSecret = "UGFBiTc8UHKhSfSY8iIfGq_Z";
            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            services.AddControllers();

            services.AddAuthorization(options =>
                options.AddPolicy("User", policy =>
                policy.Requirements.Add(new UserPolicyRequirement())
            ));

            services.AddAuthorization(options =>
            options.AddPolicy("Admin", policy =>
            policy.Requirements.Add(new AdminPolicyRequirement())
            ));

            services.AddSingleton<IAuthorizationHandler, UserPolicyHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminPolicyHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async(context, next) =>
            {
                await XRequestMiddleware(context);
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

        private Task XRequestMiddleware(HttpContext context)
        {
            return Task.Run(() =>
            {
                var request = context.Request;
                var databaseRepository = context.RequestServices.GetService<GazetaKrakowskaRepository>();

                if (request.Headers.ContainsKey("X-Request-ID"))
                {
                    if (request.Headers["X-Request-ID"].ToString().Trim() != "")
                    {
                        databaseRepository.AddLog(request.Headers["X-Request-ID"]);
                    }
                }
            });
        }
    }
}
