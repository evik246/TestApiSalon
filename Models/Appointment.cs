using System.Text.Json.Serialization;

namespace TestApiSalon.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [JsonIgnore]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        [JsonIgnore]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [JsonIgnore]
        public int ServiceId { get; set; }

        public Service? Service { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; }

        public decimal? Price { get; set; }
    }

    public enum AppointmentStatus
    {
        Active = 0,
        Completed = 1
    }
}
