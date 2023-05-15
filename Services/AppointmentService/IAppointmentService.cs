using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;

namespace TestApiSalon.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments(int customerId, int salonId, Paging paging);
        Task<Result<string>> CreateAppointment(AppointmentCreateDto request);
        Task<Result<string>> CancelAppointment(int customerId, int appointmentId);
        Task<Result<IEnumerable<MasterAppointmentDto>>> GetMasterAppintments(int masterId, Paging paging, int? customerId = null);
        Task<Result<string>> MarkAppointmentCompleted(int id);
        Task<Result<int>> GetCompletedAppointmentsCount(int masterId, DateRangeDto dateRange);
        Task<Result<double>> GetCompletedAppointmentsIncome(int masterId, DateRangeDto dateRange);
    }
}
