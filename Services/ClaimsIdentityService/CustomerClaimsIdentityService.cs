using System.Security.Claims;
using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ClaimsIdentityService
{
    public class CustomerClaimsIdentityService : IClaimsIdentityService<Customer>
    {
        public ClaimsIdentity CreateClaimsIdentity(Customer identity)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, identity.Id.ToString()),
                new Claim(ClaimTypes.Role, DbConnectionName.Client.ToString()),
                new Claim(ClaimTypes.Email, identity.Email)
            });
            return claimsIdentity;
        }
    }
}
