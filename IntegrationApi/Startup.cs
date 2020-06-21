using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using DatabaseConnection.Models;
using Extensions.Comparers;
using Implementation;
using IntegrationApi.Middlewares;
using IntegrationApi.ViewModels;
using Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Google;
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
            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));
            services.Configure<InfoViewModel>(Configuration.GetSection("Student"));
            services.AddSingleton<IWebSiteIntegration, OfertyIntegration>();
            services.AddSingleton<IDumpsRepository, DumpFileRepository>();
            services.AddSingleton<IEqualityComparer<Entry>, EntryEqualityComparer>();

            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = "Application";
                    o.DefaultSignInScheme = "External";
                })
                .AddCookie("Application")
                .AddCookie("External")
                .AddGoogle(o =>
                {
                    IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");
                    o.ClientId = googleAuthNSection["ClientId"];
                    o.ClientSecret = googleAuthNSection["ClientSecret"];
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<XRequestMiddleware>();
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
