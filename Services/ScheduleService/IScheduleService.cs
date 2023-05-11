using TestApiSalon.Dtos;

namespace TestApiSalon.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots(CustomerAppointmentDto request);
    }
}
