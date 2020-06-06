using IntegrationApi.Middlewares;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Extensions
{
    public static class ZapiszLogiExtension
    {
         public static IApplicationBuilder ZapiszLogi(this IApplicationBuilder builder)
         {
                return builder.UseMiddleware<ZapiszLogi>();
         }
        
    }
}
