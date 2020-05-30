using System.Threading.Tasks;
using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Http;

namespace IntegrationApi.Middlewares
{
    public class XRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public XRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ApplicationDbContext context)
        {
            await _next(httpContext);

            if (httpContext.Request.Headers.ContainsKey("X-Request-ID"))
            {
                context.Logs.Add(new Log(httpContext.Request.Headers["X-Request-ID"]));
                await context.SaveChangesAsync();
            }
        }
    }
}