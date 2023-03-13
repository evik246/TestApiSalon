using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos
{
    public class CustomerRegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(40, ErrorMessage = "Max length of the name is 40")]
        public required string Name { get; set; }

        [Birthday(MinAge = 1, MaxAge = 100, ErrorMessage = "Birthday is invalid")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password should be greater than or equal 8 characters")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Phone number is invalid")]
        [RegularExpression(@"\+[0-9]{12}", ErrorMessage = "Phone number is invalid")]
        public required string Phone { get; set; }
    }
}
