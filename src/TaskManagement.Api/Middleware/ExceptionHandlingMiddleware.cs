using System.Net;
using System.Text.Json;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Ocorreu um erro durante o processamento da requisição.";

            _logger.LogError(exception, "Ocorreu uma exceção: {Message}", exception.Message);

            var exceptionType = exception.GetType();

            if (exceptionType == typeof(NotFoundException))
            {
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exceptionType == typeof(ValidationException))
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exceptionType == typeof(BadRequestException))
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exceptionType == typeof(KeyNotFoundException))
            {
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exceptionType == typeof(ArgumentException) || exceptionType == typeof(ArgumentNullException))
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { message = message });
            await context.Response.WriteAsync(result);
        }
    }
}