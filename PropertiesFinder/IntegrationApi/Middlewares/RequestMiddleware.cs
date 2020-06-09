using System;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Http;


namespace IntegrationApi.Middlewares
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //public async Task Invoke(HttpContext httpContext, DatabaseContext context)
        public async Task Invoke(HttpContext httpContext)
        {
            //dodac var
            if (httpContext.Request.Headers.ContainsKey("X-Request-ID"))
            {
                // tabela w bazie ?
                using DatabaseContext databaseContext = new DatabaseContext();

                // databaseContext.Logs.Add(new Logs(httpContext.Request.Headers["X-Request-ID"]));

                databaseContext.Logs.Add(new Log()
                {
                    XRequest = httpContext.Request.Headers["X-Request-ID"],
                    Time = DateTime.Now
                });

                await databaseContext.SaveChangesAsync();
            }
            await _next(httpContext);
        
        }
    }

}

