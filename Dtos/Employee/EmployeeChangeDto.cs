using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestApiSalon.Attributes;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Employee
{
    [AtLeastOneProperty(ErrorMessage = "At least one property must be specified")]
    public class EmployeeChangeDto
    {
        [StringLength(40, ErrorMessage = "Max length of the name is 40")]
        public string? Name { get; set; }

        [StringLength(40, ErrorMessage = "Max length of the name is 40")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public string? Email { get; set; }

        [MinLength(8, ErrorMessage = "Password should be greater than or equal 8 characters")]
        public string? Password { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmployeeRole? Role { get; set; }

        [NullableProperty(nameof(IsSalonIdNullable))]
        public int? SalonId { get; set; }

        [NullableProperty(nameof(IsSpecializationNullable))]
        public string? Specialization { get; set; }

        public bool IsSalonIdNullable { get; set; } = false;

        public bool IsSpecializationNullable { get; set; } = false;
    }
}
