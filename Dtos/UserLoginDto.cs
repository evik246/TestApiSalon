using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
