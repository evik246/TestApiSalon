using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Customer
{
    public class CustomerChangePassword
    {
        [Required(ErrorMessage = "Current password is required")]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Password should be greater than or equal 8 characters")]
        public required string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
        public required string ConfirmPassword { get; set; }
    }
}
