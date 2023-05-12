using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;

namespace TestApiSalon.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments(int customerId, int salonId, Paging paging);
        Task<Result<string>> CreateAppointment(AppointmentCreateDto request);
        Task<Result<string>> CancelAppointment(int customerId, int appointmentId);
    }
}
