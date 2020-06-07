using Microsoft.AspNetCore.Builder;

namespace IntegrationApi
{
    public static class LoggerExtension
    {
        public static IApplicationBuilder UseLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Logger>();
        }
    }
}
