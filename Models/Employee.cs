using System.Text.Json.Serialization;

namespace TestApiSalon.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [JsonIgnore]
        public int? SalonId { get; set; }

        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? PhotoName { get; set; }

        public string? Specialization { get; set; }
    }
}
