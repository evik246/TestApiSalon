using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class UserLoginDto
    {
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
