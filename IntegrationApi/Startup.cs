using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection.Interfaces;
using DatabaseConnection.Services;
using Interfaces;
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
using ZnajdzTo;

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
            services.AddControllers();

            services.AddSingleton<IWebSiteIntegration, ZnajdzToIntegration>();
            services.AddSingleton<IDumpsRepository, DumpFileRepository>();
            services.AddSingleton<IEqualityComparer<Entry>, ZnajdzToComparer>();
            services.AddSingleton<IDatabaseService, DatabaseService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGoogle(options =>
            {
               options.ClientId = "245258053536-jdkribc08ids5n4g50fdgag0des7rjcr.apps.googleusercontent.com";
               options.ClientSecret = "Oxg7ydpsteIR5LU2k9D8pkNb";
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            services.AddAuthorization(options =>
                options.AddPolicy("User", policy =>
                   policy.Requirements.Add(new UserPolicyRequirement())
                )
            );
            services.AddAuthorization(options =>
               options.AddPolicy("Admin", policy =>
                   policy.Requirements.Add(new AdminPolicyRequirement())
               )
            );
            services.AddSingleton<IAuthorizationHandler, UserPolicyHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminPolicyHandler>();
        }

        public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement>
        {
            const string GoogleEmailAddressSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            protected override Task HandleRequirementAsync( AuthorizationHandlerContext context,
                                                            AdminPolicyRequirement requirement)
            {
                var email = context.User.Claims.FirstOrDefault(p => p.Issuer.Equals("Google") &&
                                                                    p.Type.Equals(GoogleEmailAddressSchema)
                );
                if (email != null && (email.Value.Equals("s170633pn@gmail.com") || 
                                      email.Value.Equals("piotr.tybura@gmail.com")))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }
        public class AdminPolicyRequirement : IAuthorizationRequirement { }
        public class UserPolicyHandler : AuthorizationHandler<UserPolicyRequirement>
        {
            protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserPolicyRequirement requirement)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        public class UserPolicyRequirement : IAuthorizationRequirement { }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware();

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
