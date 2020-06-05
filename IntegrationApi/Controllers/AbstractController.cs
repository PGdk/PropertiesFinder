using System;
using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    public abstract class AbstractController : ControllerBase
    {
        private static readonly string LogKey = "X-Request-ID";

        protected void GenerateLog()
        {
            if (Request.Headers.ContainsKey(LogKey) && !string.IsNullOrEmpty(Request.Headers[LogKey]))
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    context.Logs.Add(
                        new Log
                        {
                            Value = Request.Headers[LogKey],
                            CreatedAt = DateTime.Now
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}
