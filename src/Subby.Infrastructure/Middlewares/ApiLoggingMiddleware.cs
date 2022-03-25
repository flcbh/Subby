using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Subby.Core.Entities;
using Subby.Core.Extensions;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Subby.Infrastructure.Middlewares
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private IRepository _repository;
        private readonly ILogger _logger;
        public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, IRepository repository)
        {
            try
            {
                _repository = repository;
                var request = httpContext.Request;
                if (request.Path.StartsWithSegments(new PathString("/api")))
                {
                    var stopWatch = Stopwatch.StartNew();
                    var requestTime = DateTime.UtcNow;
                    var requestBodyContent = await ReadRequestBody(request);
                    var originalBodyStream = httpContext.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        var response = httpContext.Response;
                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);
                        await responseBody.CopyToAsync(originalBodyStream);
                        // user agent
                        var userAgent = request.Headers["User-Agent"];
                        // browser version
                        var browserVersion = "";
                        // client ip address
                        var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();

                        var requestBodyData = requestBodyContent;
                        var responseBodyData = responseBodyContent;
                        if (request.Path.ToString().ToLower().StartsWith("/api/login"))
                        {
                            requestBodyData = "(Request logging disabled for /api/login)";
                            responseBodyData = "(Response logging disabled for /api/login)";
                        }
            
                        try
                        {
                            _repository.Add(new Audit
                            {
                                Description = "",
                                IpAddress = ipAddress,
                                Path = request.Path,
                                UserAgent = userAgent,
                                TenantId = httpContext.User.Identity.GetApplicationId(),
                                TenantName = httpContext.User.Identity.GetApplicationName(),
                                Sandbox = httpContext.User.Identity.Sandbox(),
                                RequestTime = requestTime,
                                ResponseMillis = stopWatch.ElapsedMilliseconds,
                                StatusCode = response.StatusCode,
                                Method = request.Method,
                                QueryString = request.QueryString.ToString(),
                                RequestBody = requestBodyData,
                                ResponseBody = responseBodyData,
                                Browser = browserVersion,
                            });

                        } catch (Exception ex)
                        {
                            _logger.LogError(ex,"User activity logging error");
                        }
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "API Logging Error", ex.Message);
                await _next(httpContext);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }
    }

    public static class ApiLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLoggingMiddleware>();
        }
    }
}