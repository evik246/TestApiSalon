using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Service
{
    public class ServiceAppointmentAccount
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public ServiceCategory Category { get; set; } = new();

        public long AppointmentsCount { get; set; }
    }
}
