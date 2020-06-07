using DatabaseConnection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, DatabaseContext databaseContext)
        {
            databaseContext.Add(new RequestLog()
            {
                Header = httpContext.Request.Headers["X-Request-ID"],
                Timestamp = DateTime.Now
            });
            databaseContext.SaveChanges();
            await _next(httpContext);
        }
    }
}
