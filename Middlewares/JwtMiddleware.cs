using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using TestApiSalon.Services;

namespace TestApiSalon.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (token != null && tokenService.ValidateToken(token))
            {
                var claimsIdentity = tokenService.GetIdentity(token);
                if (claimsIdentity != null)
                {
                    context.User = new ClaimsPrincipal(claimsIdentity);
                }
            }
            Console.WriteLine($"Authorization header: {token}");
            await _next(context);
        }
    }
}
