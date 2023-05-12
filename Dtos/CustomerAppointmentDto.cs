using TestApiSalon.Models;

namespace TestApiSalon.Dtos
{
    public class CustomerAppointmentDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public ServiceDto Service { get; set; } = new ServiceDto();

        public MasterDto Master { get; set; } = new MasterDto();
    }
}
