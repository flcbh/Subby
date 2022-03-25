using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Subby.Utilities.Middleware
{
    public class SwaggerContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public SwaggerContextMiddleware(RequestDelegate next, ILogger<SwaggerContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);
        }
    }

    public static class SwaggerContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder builder, string appName, decimal version)
        {
            builder.UseSwagger();
            builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v{version}/swagger.json", appName + $" v{version}");
            });
            return builder.UseMiddleware<SwaggerContextMiddleware>();
        }
    }
}
