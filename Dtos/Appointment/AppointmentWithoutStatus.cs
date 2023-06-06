using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Service;

namespace TestApiSalon.Dtos.Appointment
{
    public class AppointmentWithoutStatus
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public CustomerDto Customer { get; set; } = new();

        public MasterDto Master { get; set; } = new();

        public ServiceNameDto Service { get; set; } = new();

        public decimal? Price { get; set; }
    }
}
