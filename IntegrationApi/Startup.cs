using Microsoft.AspNetCore.Builder;
using DatabaseConnection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DatabaseConnection.Models;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace IntegrationApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public class RequestLoggingMiddleware {
            private readonly RequestDelegate _next;
            private readonly ILogger _logger;

            public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory) {
                _next = next;
                _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
            }

            public async Task Invoke(HttpContext context) {
                if ( context.Request.Headers.ContainsKey("X-Request-ID") ) {
                    var database = context.RequestServices.GetService<MorizonContext>();

                    Log log = new Log() { DateTime = DateTime.Now, Value = context.Request.Headers["X-Request-ID"] };
                    database.Logs.Add(log);

                    await database.SaveChangesAsync();
                }

                await _next(context);
            }

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            services.AddDbContext<MorizonContext>(options =>
                options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=joannal165459;Integrated Security=True"));

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddGoogle(options => {
                options.ClientId = "172968747584-v9atbr3nddfbconulntaglcgtc0as651.apps.googleusercontent.com";
                options.ClientSecret = "3Qaor472uordTd3yfDtgjiT-";
            })
             .AddCookie(options => {

                 options.Cookie.HttpOnly = false;
                 options.Cookie.SameSite = SameSiteMode.None;
             });

            services.AddAuthorization(options =>
                        options.AddPolicy("User", policy =>
                                policy.Requirements.Add(new UserPolicyRequirement())));
            services.AddAuthorization(options =>
                        options.AddPolicy("Admin", policy =>
                                policy.Requirements.Add(new AdminPolicyRequirement())));

            services.AddSingleton<IAuthorizationHandler, UserPolicyHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminPolicyHandler>();
            services.AddMvc();

        }

        public class AdminPolicyHandler : AuthorizationHandler<AdminPolicyRequirement> {
            const string GoogleEmailAddressSchema =
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminPolicyRequirement requirement) {
                var email = context.User.Claims.FirstOrDefault(p =>
                p.Issuer.Equals("Google") &&
                p.Type.Equals(GoogleEmailAddressSchema));
                if ( email != null && ( email.Value.Equals("piotr.tybura@gmail.com") || email.Value.Equals("joannaxlitwin@gmail.com") ))
                    context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        public class AdminPolicyRequirement : IAuthorizationRequirement { }
        public class UserPolicyHandler : AuthorizationHandler<UserPolicyRequirement> {
            protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserPolicyRequirement requirement) {
                if ( context.User.Identity.IsAuthenticated )
                    context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        public class UserPolicyRequirement : IAuthorizationRequirement { }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
