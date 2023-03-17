using System.Net;
using System.Text.Json;
using TestApiSalon.Exceptions;

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
                _logger.LogError($"Something went wrong: {ex}");

                var response = context.Response;
                response.ContentType = "application/json";
                string message = ex.Message;

                switch (ex)
                {
                    case UnauthorizedException:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case ConflictException:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        break;
                    case NotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ForbiddenException:
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = "Internal Server Error: " + ex.Message;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = message });
                await response.WriteAsync(result);
            }
        }
    }
}
