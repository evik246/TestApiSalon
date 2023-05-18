using System.Security.Claims;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ClaimsIdentityService
{
    public class UserClaimsIdentityService : IClaimsIdentityService<User>
    {
        public ClaimsIdentity CreateClaimsIdentity(User identity)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, identity.Role.ToString()),
                new Claim(ClaimTypes.Email, identity.Email)
            });
            return claimsIdentity;
        }
    }
}
