using TestApiSalon.Dtos;

namespace TestApiSalon.Services.AuthService
{
    public interface IAuthService
    {
        Task<string?> Login(UserLoginDto request);
        Task<bool> ResetPassword(UserUpdatePasswordDto request);
    }
}
