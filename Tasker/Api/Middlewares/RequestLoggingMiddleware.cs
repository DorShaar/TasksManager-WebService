using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Tasker.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate mNext;
        private readonly ILogger<RequestLoggingMiddleware> mLogger;

        public RequestLoggingMiddleware(RequestDelegate requestDelegate, ILogger<RequestLoggingMiddleware> logger)
        {
            mNext = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await mNext.Invoke(context).ConfigureAwait(false);
            }
            finally
            {
                mLogger.LogDebug(
                    $"Request method: {context.Request?.Method}{Environment.NewLine}" +
                    $"Request host: {context.Request?.Host.Host}{Environment.NewLine}" +
                    $"Request host port: {context.Request?.Host.Port}{Environment.NewLine}" +
                    $"Request path: {context.Request?.Path.Value}{Environment.NewLine}" +
                    $"Request status: {context.Response?.StatusCode}{Environment.NewLine}");
            }
        }
    }
}