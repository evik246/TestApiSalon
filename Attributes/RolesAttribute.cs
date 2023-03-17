using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Security.Claims;
using TestApiSalon.Exceptions;

namespace TestApiSalon.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            if (httpContext == null || httpContext.User == null || httpContext.User.Identity == null)
            {
                throw new UnauthorizedException("Bearer token is missed");
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException("Bearer token is missed");
            }

            ClaimsIdentity identity = (ClaimsIdentity)httpContext.User.Identity;

            var roles = identity.Claims
                .Where(c => c.Type.Equals("role"))
                .Select(c => c.Value)
                .ToArray();

            if (_roles.Any() && !roles.Any(r => _roles.Contains(r)))
            {
                throw new ForbiddenException("No permission to access");
            }
        }
    }
}
