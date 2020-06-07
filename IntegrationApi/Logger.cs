using System;
using System.Threading.Tasks;
using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Http;

namespace IntegrationApi
{
    public class Logger
    {
        private static readonly string LogKey = "X-Request-ID";

        private readonly RequestDelegate _next;

        public Logger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(LogKey) && !string.IsNullOrEmpty(context.Request.Headers[LogKey]))
            {
                using (DatabaseContext databaseContext = new DatabaseContext())
                {
                    databaseContext.Logs.Add(
                        new Log
                        {
                            Value = context.Request.Headers[LogKey],
                            CreatedAt = DateTime.Now
                        }
                    );

                    await databaseContext.SaveChangesAsync();
                }
            }

            await _next(context);
        }
    }
}
