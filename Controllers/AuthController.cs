using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos.Auth;
using TestApiSalon.Extensions;
using TestApiSalon.Services.AuthService;

namespace TestApiSalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var token = await _authService.Login(request);
            return token.MakeResponse();
        }

        [HttpPut("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserUpdatePasswordDto request)
        {
            var result = await _authService.ResetPassword(request);
            return result.MakeResponse();
        }
    }
}
