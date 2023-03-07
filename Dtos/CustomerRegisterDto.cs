using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class CustomerRegisterDto
    {
        [Required]
        public required string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public required string Phone { get; set; }
    }
}
