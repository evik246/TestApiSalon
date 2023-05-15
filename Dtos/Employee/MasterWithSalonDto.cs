using System.Text.Json.Serialization;
using TestApiSalon.Dtos.Salon;

namespace TestApiSalon.Dtos.Employee
{
    public class MasterWithSalonDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhotoPath { get; set; }

        public string? Specialization { get; set; }

        [JsonIgnore]
        public int SalonId { get; set; }

        public SalonDto? Salon { get; set; }
    }
}
