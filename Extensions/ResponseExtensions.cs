using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Net.Mime;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Exceptions;

namespace TestApiSalon.Extensions
{
    public static class ResponseExtensions
    {
        public static Result<int> GetAuthorizedUserId(this ControllerBase controller)
        {
            string? stringId = controller.User.Claims.FirstOrDefault(
                c => c.Type.Equals("nameid"))?.Value;
            
            if (int.TryParse(stringId, out int id))
            {
                return new Result<int>(id);
            }
            return new Result<int>(new ForbiddenException("No permission to access"));
        }

        public static Result<int> GetAuthorizedEmployeeSalonId(this ControllerBase controller)
        {
            string? stringSalonId = controller.User.Claims.FirstOrDefault(
                c => c.Type.Equals("salonid"))?.Value;

            if (int.TryParse(stringSalonId, out int salonId))
            {
                return new Result<int>(salonId);
            }
            return new Result<int>(new ForbiddenException("No permission to access"));
        }

        public static IActionResult MakeFileResponse(this Result<Stream> result, ControllerBase controllerBase)
        {
            return result.Match(stream =>
            {
                return controllerBase.File(stream, MediaTypeNames.Image.Jpeg);
            }, exception =>
            {
                return result.MakeResponse();
            });
        }

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
                if (p.SqlState.Equals("23503"))
                {
                    if (!string.IsNullOrEmpty(p.ConstraintName))
                    {
                        string tableName = p.ConstraintName.Split('_')[1];

                        string message = tableName switch
                        {
                            "category" => "Service category is not found",
                            "city" => "City is not found",
                            "salon" => "Salon is not found",
                            "customer" => "Customer is not found",
                            "employee" => "Employee is not found",
                            "service" => "Service is not found",
                            _ => throw new NotImplementedException(),
                        };
                        return new NotFoundException(message);
                    }
                }
            }
            return exception;
        }
    }
}
