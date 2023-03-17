using System.Security.Claims;

namespace TestApiSalon.Services
{
    public interface IClaimsIdentityService<T>
    {
        ClaimsIdentity CreateClaimsIdentity(T identity);
    }
}
