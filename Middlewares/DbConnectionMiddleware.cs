using System.Security.Claims;
using TestApiSalon.Data;
using TestApiSalon.Extensions;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Middlewares
{
    public class DbConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public DbConnectionMiddleware(RequestDelegate next, ILogger<DbConnectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IDbConnectionService connectionManager)
        {
            try
            {
                var identity = context.User.Identity as ClaimsIdentity;

                var role = identity?.FindFirst("role")?.Value;

                connectionManager.ConnectionName = role switch
                {
                    "Admin" => DbConnectionName.Admin,
                    "Manager" => DbConnectionName.Manager,
                    "Master" => DbConnectionName.Master,
                    "Client" => DbConnectionName.Client,
                    _ => DbConnectionName.Client,
                };
                await _next(context);
            }
            catch (Exception ex)
            {
                const string message = "An unhandled exception has occurred while executing the request.";
                _logger.LogError(ex, message);
                await context.Response.MakeResponse(ex);
            }
        }
    }
}
