using System.Security.Claims;
using TestApiSalon.Services;

namespace TestApiSalon.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token) && tokenService.ValidateToken(token))
            {
                var claimsIdentity = tokenService.GetIdentity(token);
                if (claimsIdentity != null)
                {
                    context.User = new ClaimsPrincipal(claimsIdentity);
                }
            }
            _logger.LogInformation($"Authorization header: {token}");
            await _next(context);
        }
    }
}
