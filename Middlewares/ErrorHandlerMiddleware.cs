using Npgsql;
using TestApiSalon.Exceptions;
using TestApiSalon.Extensions;

namespace TestApiSalon.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
                _logger.LogError(ex, $"Error occurred while processing the request: {context.Request.Path}");

                await context.Response.MakeResponse(ex);
            }
        }
    }
}
