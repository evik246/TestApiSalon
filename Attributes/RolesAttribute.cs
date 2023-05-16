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
            var user = context.HttpContext?.User;
            var identity = user?.Identity as ClaimsIdentity;

            if (user == null ||  identity == null || !identity.IsAuthenticated) 
            {
                if (_roles.Length == 0 || _roles.Contains("Guest")) 
                {
                    return;
                }
                throw new UnauthorizedException("Bearer token is missed");
            }

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
