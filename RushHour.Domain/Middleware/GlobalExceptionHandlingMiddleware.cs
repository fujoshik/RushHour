using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using RushHour.Domain.Exceptions;

namespace RushHour.Domain.Middleware
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        ILogger<GlobalExceptionHandlingMiddleware> _logger;
        public GlobalExceptionHandlingMiddleware(
            ILogger<GlobalExceptionHandlingMiddleware> logger)
            => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);                              
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            _logger.LogError(e, e.Message);

            int status;
            string message = "";

            var exceptionType = e.GetType();

            if (exceptionType == typeof(KeyNotFoundException))
            {
                status = (int)HttpStatusCode.NotFound;
                message = e.Message;
            }
            else if (exceptionType == typeof(ArgumentNullException))
            {
                status = (int)HttpStatusCode.NotFound;
                message = e.Message;
            }
            else if (exceptionType == typeof(ArgumentNullException))
            {
                status = (int)HttpStatusCode.NotFound;
                message = e.Message;
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                status = (int)HttpStatusCode.Unauthorized;
                message = e.Message;
            }
            else if (exceptionType == typeof(ArgumentOutOfRangeException))
            {
                status = (int)HttpStatusCode.BadRequest;
                message = e.Message;
            }
            else
            {
                status = (int)HttpStatusCode.InternalServerError;
                message = e.Message;
            }

            context.Response.StatusCode = (int)status;

            context.Response.ContentType = "application/json";

            Error error = new Error { StatusCode = status.ToString(), Message = message };

            await context.Response.WriteAsync(error.ToString());
        }
    }
}
