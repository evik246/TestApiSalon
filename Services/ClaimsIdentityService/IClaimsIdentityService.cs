using System.Security.Claims;

namespace TestApiSalon.Services.ClaimsIdentityService
{
    public interface IClaimsIdentityService<T>
    {
        ClaimsIdentity CreateClaimsIdentity(T identity);
    }
}
