using Logger.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Tasker.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {

        private readonly RequestDelegate mNext;
        private readonly ILogger mLogger;

        public RequestLoggingMiddleware(RequestDelegate requestDelegate, ILogger logger)
        {
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            mNext = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await mNext.Invoke(context).ConfigureAwait(false);
            }
            finally
            {
                mLogger.LogInformation(
                    $"Request method: {context.Request?.Method} " +
                    $"Request path: {context.Request?.Path.Value}" +
                    $"Request status: {context.Response?.StatusCode}");
            }
        }
    }
}