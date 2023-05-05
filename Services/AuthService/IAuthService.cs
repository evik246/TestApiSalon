using TestApiSalon.Dtos;

namespace TestApiSalon.Services.AuthService
{
    public interface IAuthService
    {
        Task<Result<string>> Login(UserLoginDto request);
        Task<Result<string>> ResetPassword(UserUpdatePasswordDto request);
    }
}
