using System.Security.Claims;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ClaimsIdentityService
{
    public class EmployeeClaimsIdentityService : IClaimsIdentityService<Employee>
    {
        public ClaimsIdentity CreateClaimsIdentity(Employee identity)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, identity.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, identity.Id.ToString())
            });

            if (identity.SalonId != null)
            {
                claimsIdentity.AddClaim(new Claim("salonid", identity.SalonId.ToString()!));
            }
            return claimsIdentity;
        }
    }
}
