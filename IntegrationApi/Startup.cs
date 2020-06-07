using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseConnection;
using IntegrationApi.Middleware;
using IntegrationApi.Repositories;
using IntegrationApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using NieruchomosciOnline;
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
            services.AddDbContext<DatabaseContext>();
            IMapper mapper = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile())).CreateMapper();

            services.AddTransient<ILogsRepository, LogsRepository>();
            services.AddTransient<IEntriesRepository, EntriesRepository>();
            services.AddTransient<IEntriesService, EntriesService>();
            services.AddTransient<NieruchomosciOnlineIntegration, NieruchomosciOnlineIntegration>();
            services.AddTransient<IDumpsRepository, DumpFileRepository>();
            services.AddTransient<IEqualityComparer<Entry>, NieruchomosciOnlineComparer>();
            services.AddSingleton(mapper);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<RequestIdMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
