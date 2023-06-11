using System.ComponentModel;
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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmployeeRole Role { get; set; }

        [JsonIgnore]
        public string? PhotoPath { get; set; }

        public string? PhotoURL { get; set; }

        public string? Specialization { get; set; }
    }

    public enum EmployeeRole
    {
        [Description("Master")]
        Master = 0,
        [Description("Manager")]
        Manager = 1,
        [Description("Admin")]
        Admin = 2
    }
}
