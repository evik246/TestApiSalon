using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
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
        public async Task<IActionResult> GetFreeSlots([FromQuery] CustomerAppointmentDto request)
        {
            var slots = await _scheduleService.GetAvailableTimeSlots(request);
            return slots.MakeResponse();
        }
    }
}
