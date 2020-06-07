using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IntegrationApi
{
    public class XRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public XRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);

            BezposrednieIntegrationRepo repo = new BezposrednieIntegrationRepo();

            if (httpContext.Request.Headers.ContainsKey("X-Request-ID"))
            {
                repo.AddLog(httpContext.Request.Headers["X-Request-ID"]);
                await _next(httpContext);
            }
        }
    }
}
