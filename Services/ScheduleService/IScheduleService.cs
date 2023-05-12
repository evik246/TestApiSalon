using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots(CustomerAppointmentDto request);
        Task<Result<IEnumerable<Schedule>>> GetMasterSchedule(int employeeId);
        Task<Result<IEnumerable<MasterWorkingDayDto>>> GetMasterWorkingDays(int employeeId, DateRangeDto dateRange);
    }
}
