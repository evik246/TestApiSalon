using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;

namespace TestApiSalon.Extensions
{
    public static class ResponseExtensions
    {
        public static IActionResult MakeResponse<TResult>(this Result<TResult> result)
        {
            return result.Match(obj =>
            {
                return new OkObjectResult(obj);
            }, exception =>
            {
                exception = MakeReadable(exception);

                var message = new { message = exception.Message };

                IActionResult response = exception switch
                {
                    UnauthorizedException => new UnauthorizedObjectResult(message),
                    ConflictException => new ConflictObjectResult(message),
                    NotFoundException => new NotFoundObjectResult(message),
                    ForbiddenException => new ObjectResult(message) { StatusCode = 403},
                    _ => new ObjectResult(message) { StatusCode = 500}
                };
                return response;
            });
        }

        public static async Task MakeResponse(this HttpResponse response, Exception exception)
        {
            exception = MakeReadable(exception);

            var message = new { message = exception.Message };

            response.StatusCode = exception switch
            {
                UnauthorizedException => (int) HttpStatusCode.Unauthorized,
                ConflictException => (int) HttpStatusCode.Conflict,
                NotFoundException => (int) HttpStatusCode.NotFound,
                ForbiddenException => (int) HttpStatusCode.Forbidden,
                _ => (int) HttpStatusCode.InternalServerError
            };

            await response.WriteAsJsonAsync(message);
        }

        private static Exception MakeReadable(Exception exception)
        {
            if (exception is PostgresException p)
            {
                if (p.SqlState.Equals("42501"))
                {
                    return new ForbiddenException("No permission to access");
                }
            }
            return exception;
        }
    }
}
