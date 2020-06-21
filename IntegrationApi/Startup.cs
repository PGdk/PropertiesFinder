using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DatabaseConnection;
using Microsoft.AspNetCore.Http;
using Utilities;
using Models;
using Application.Otodom;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;



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
            services.AddSingleton(typeof(IOtodomRepository), new OtodomRepository());
            services.AddSingleton(typeof(IDumpsRepository), new DumpFileRepository());
            services.AddSingleton(typeof(IEqualityComparer<Entry>), new OtodomComparer());

            services.AddControllers();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private Task XRequestIDMiddleware(HttpContext context)
        {
            return Task.Run(() =>
            {
                var request = context.Request;
                var databaseRepository = context.RequestServices.GetService<IOtodomRepository>();

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
