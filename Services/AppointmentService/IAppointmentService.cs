using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments(int customerId, int salonId, Paging paging);
    }
}
