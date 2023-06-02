using System.Security.Claims;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ClaimsIdentityService
{
    public class CustomerClaimsIdentityService : IClaimsIdentityService<Customer>
    {
        public ClaimsIdentity CreateClaimsIdentity(Customer identity)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Client"),
                new Claim(ClaimTypes.NameIdentifier, identity.Id.ToString()),
                new Claim("email", identity.Email)
            });
            return claimsIdentity;
        }
    }
}
