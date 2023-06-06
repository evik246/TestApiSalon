using System.Text.Json.Serialization;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Service;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Appointment
{
    public class ManagerAppointmentDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public CustomerDto Customer { get; set; } = new();

        public MasterDto Master { get; set; } = new();

        public ServiceNameDto Service { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }

        public decimal? Price { get; set; }
    }
}
