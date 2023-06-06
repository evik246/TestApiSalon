using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;

namespace TestApiSalon.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments(int customerId, int salonId, Paging paging);
        Task<Result<string>> CreateAppointment(int customerId, AppointmentCreateDto request);
        Task<Result<string>> CancelAppointment(int customerId, int appointmentId);
        Task<Result<IEnumerable<MasterAppointmentDto>>> GetMasterAppintments(int masterId, Paging paging, int? customerId = null);
        Task<Result<string>> MarkMasterAppointmentCompleted(int masterId, int appointmentId);
        Task<Result<string>> MarkManagerAppointmentCompleted(int salonId, int appointmentId);
        Task<Result<int>> GetCompletedAppointmentsCount(int masterId, DateRangeDto dateRange);
        Task<Result<double>> GetCompletedAppointmentsIncome(int masterId, DateRangeDto dateRange);
        Task<Result<CustomerAppointmentDto>> GetCustomerAppointmentById(int customerId, int appointmentId);
        Task<Result<IEnumerable<ManagerAppointmentDto>>> GetManagerAppointments(int salonId, Paging paging);
        Task<Result<IEnumerable<AppointmentWithoutStatus>>> GetManagerActiveAppointments(int salonId, Paging paging);
        Task<Result<IEnumerable<AppointmentWithoutStatus>>> GetManagerCompletedAppointments(int salonId, Paging paging);
        Task<Result<IEnumerable<AppointmentWithoutStatus>>> GetManagerUncompletedAppointments(int salonId, Paging paging);
        Task<Result<string>> ChangeMasterInAppointment(int salonId, int appointmentId, int masterId);
        Task<Result<ManagerAppointmentDto>> GetManagerAppointmentById(int salonId, int appointmentId);
    }
}
