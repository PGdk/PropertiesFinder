using DatabaseConnection;
using IntegrationApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Middlewares
{
    public class ZapiszLogi
    {
        private readonly RequestDelegate _next;
        public ZapiszLogi(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Headers.ContainsKey("X-Request-ID"))
            {
                ObslugaBazyDanych.ZapiszLogi(context.Request.Headers["X-Request-ID"]);
            }
            await _next.Invoke(context);
        }
    }
}
