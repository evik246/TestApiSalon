using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots(CustomerAppointmentInputDto request);
        Task<Result<IEnumerable<Schedule>>> GetMasterSchedule(int employeeId);
        Task<Result<IEnumerable<MasterWorkingDayDto>>> GetMasterWorkingDays(int employeeId, DateRangeDto dateRange);
    }
}
