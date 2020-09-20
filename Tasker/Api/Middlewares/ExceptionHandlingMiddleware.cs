using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Tasker.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate mNext;
        private readonly ILogger<ExceptionHandlingMiddleware> mLogger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                mLogger.LogError($"Handling exception at {nameof(ExceptionHandlingMiddleware)}");
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            Type exceptionType = exception.GetType();

            return exceptionType == typeof(ValidationException)
                ? Generate400Response(context, exception)
                : Generate500Response(context, exception);
        }

        private Task Generate500Response(HttpContext context, Exception exception)
        {
            mLogger.LogError(exception, "Unhandled exception occured during request pipeline");

            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.com/500",
                Title = "The server encountered an unexpected condition that prevented it from fulfilling the request",
                Detail = $"Trace id {context.TraceIdentifier}",
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }

        private Task Generate400Response(HttpContext context, Exception exception)
        {
            mLogger.LogError(exception, exception.Message);

            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatuses.com/400",
                Title = "Bad request error",
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }
    }
}