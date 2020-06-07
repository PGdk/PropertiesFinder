using DatabaseConnection.Models;
using IntegrationApi.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace IntegrationApi.Middleware
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate next;

        public RequestIdMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ILogsRepository logsRepository)
        {
            if (context.Request.Headers.ContainsKey("X-Request-ID"))
            {
                logsRepository.AddLog(new Log()
                {
                    RequestId = context.Request.Headers["X-Request-ID"],
                    TimeStamp = DateTime.Now
                });
            }

            await this.next(context);
        }
    }
}
