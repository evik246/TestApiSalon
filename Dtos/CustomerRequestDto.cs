using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class CustomerRequestDto
    {
        public required string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }

        public required string Password { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
        public required string Phone { get; set; }
    }
}
