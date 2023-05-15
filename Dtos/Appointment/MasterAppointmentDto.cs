using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Service;

namespace TestApiSalon.Dtos.Appointment
{
    public class MasterAppointmentDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public ServiceNameDto Service { get; set; } = new ServiceNameDto();

        public CustomerDto Customer { get; set; } = new CustomerDto();
    }
}
