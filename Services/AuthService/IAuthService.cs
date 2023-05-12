using TestApiSalon.Dtos.Auth;
using TestApiSalon.Dtos.Other;

namespace TestApiSalon.Services.AuthService
{
    public interface IAuthService
    {
        Task<Result<string>> Login(UserLoginDto request);
        Task<Result<string>> ResetPassword(UserUpdatePasswordDto request);
    }
}
