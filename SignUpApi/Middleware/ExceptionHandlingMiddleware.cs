namespace SignUpApi.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Application.CustomExceptions;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UserAlreadyExistsException => HttpStatusCode.Conflict,       // 409 Conflict
                ValidationException => HttpStatusCode.BadRequest,            // 400 Bad Request
                NotFoundException => HttpStatusCode.NotFound,                // 404 Not Found
                _ => HttpStatusCode.InternalServerError                      // Default 500
            };

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                context.Response.StatusCode,
                exception.Message
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }


}
