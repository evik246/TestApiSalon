using System.Text.Json.Serialization;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Employee
{
    public class EmployeeFiltrationDto
    {
        public int? SalonId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmployeeRole? Role { get; set;}
    }
}
