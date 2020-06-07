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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
