using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
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
        public async Task<ActionResult<string>> Login([FromBody]UserLoginDto request)
        {
            var token = await _authService.Login(request) 
                ?? throw new UnauthorizedException("Invalid email or password");
            return Ok(token);
        }

        [HttpPut("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserUpdatePasswordDto request)
        {
            return await _authService.ResetPassword(request) 
                ? Ok("Password is changed") 
                : throw new UnauthorizedException("Invalid email or password");
        }
    }
}
