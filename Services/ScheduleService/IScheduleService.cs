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
        Task<Result<IEnumerable<Schedule>>> GetManagerMasterSchedule(int salonId, int masterId);
        Task<Result<IEnumerable<MasterWorkingDayDto>>> GetMasterWorkingDays(int employeeId, DateRangeDto dateRange);
        Task<Result<string>> ChangeManagerMasterSchedule(int salonId, int scheduleId, MasterScheduleChangeDto request);
        Task<Result<string>> SetManagerMasterSchedule(int salonId, int masterId, MasterScheduleCreateDto request);
        Task<Result<string>> DeleteManagerMasterSchedule(int salonId, int scheduleId);
    }
}
