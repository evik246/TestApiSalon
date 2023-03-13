using System.Net;
using TestApiSalon.Data;

namespace TestApiSalon.Middlewares
{
    public class DbConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public DbConnectionMiddleware(RequestDelegate next, ILogger<IDbConnectionManager> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IDbConnectionManager connectionManager)
        {
            try
            {
                connectionManager.ConnectionName = DbConnectionName.Client;
                await _next(context);
            }
            catch (Exception ex)
            {
                const string message = "An unhandled exception has occurred while executing the request.";
                _logger.LogError(ex, message);
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
