using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Auth
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
