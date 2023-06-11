using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Employee
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required EmployeeRole Role { get; set; }

        public int? SalonId { get; set; }
    }
}
