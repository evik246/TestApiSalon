using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Schedule;
using TestApiSalon.Extensions;
using TestApiSalon.Services.ScheduleService;

namespace TestApiSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("timeslots/available")]
        public async Task<IActionResult> GetFreeSlots([FromQuery] CustomerAppointmentInputDto request)
        {
            var slots = await _scheduleService.GetAvailableTimeSlots(request);
            return slots.MakeResponse();
        }

        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterSchedule(int id)
        {
            var schedule = await _scheduleService.GetMasterSchedule(id);
            return schedule.MakeResponse();
        }

        [HttpGet("master/{id}/working_days")]
        public async Task<IActionResult> GetMasterWorkingDays(int id, [FromQuery] DateRangeDto dateRange)
        {
            var days = await _scheduleService.GetMasterWorkingDays(id, dateRange);
            return days.MakeResponse();
        }
    }
}
