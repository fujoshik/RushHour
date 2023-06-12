using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
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
                _logger.LogError(e, e.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                var error = new Error()
                {
                    StatusCode = context.Response.StatusCode.ToString(),
                    Message = e.Message
                };

                await context.Response.WriteAsync(error.ToString());               
            }
        }
    }
}
