using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace IntegrationApi
{
    public class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("X-Request-ID"))
            {
                using DatabaseContext databaseContext = new DatabaseContext();
                databaseContext.Logs.Add(new Logs()
                {
                    XRequestID = httpContext.Request.Headers["X-Request-ID"],
                    Time = DateTime.Now
                });
                await databaseContext.SaveChangesAsync();
            }

            await _next(httpContext);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware>();
        }
    }
}
