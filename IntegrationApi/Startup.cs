using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DatabaseConnection;
using Microsoft.AspNetCore.Http;
using Utilities;
using Models;
using Application.Trovit;
using System.Collections.Generic;

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
            services.AddSingleton(typeof(ITrovitRepository), new TrovitRepository());
            services.AddSingleton(typeof(IDumpsRepository), new DumpFileRepository());
            services.AddSingleton(typeof(IEqualityComparer<Entry>), new TrovitComparer());

            services.AddControllers();

            services.AddMvc();
        }
        private Task XRequestIDMiddleware(HttpContext context)
        {
            return Task.Run(() =>
            {
                var request = context.Request;
                var databaseRepository = context.RequestServices.GetService<ITrovitRepository>();

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