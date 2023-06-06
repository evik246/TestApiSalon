using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;
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

        [Roles("Client")]
        [HttpGet("timeslots/available")]
        public async Task<IActionResult> GetFreeSlots([FromQuery] CustomerAppointmentInputDto request)
        {
            var slots = await _scheduleService.GetAvailableTimeSlots(request);
            return slots.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterSchedule(int id)
        {
            var schedule = await _scheduleService.GetMasterSchedule(id);
            return schedule.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account")]
        public async Task<IActionResult> GetMasterScheduleAccount()
        {
            var employeeId = this.GetAuthorizedUserId();
            if (employeeId.State == ResultState.Success)
            {
                var schedule = await _scheduleService.GetMasterSchedule(employeeId.Value);
                return schedule.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/master/{masterId}")]
        public async Task<IActionResult> GetManagerMasterSchedule(int masterId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var schedule = await _scheduleService.GetManagerMasterSchedule(salonId.Value, masterId);
                return schedule.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Client")]
        [HttpGet("master/{id}/working_days")]
        public async Task<IActionResult> GetMasterWorkingDays(int id, [FromQuery] DateRangeDto dateRange)
        {
            var days = await _scheduleService.GetMasterWorkingDays(id, dateRange);
            return days.MakeResponse();
        }

        [Roles("Manager")]
        [HttpPut("{scheduleId}/manager/account")]
        public async Task<IActionResult> ChangeManagerMasterSchedule(int scheduleId, [FromBody] MasterScheduleChangeDto request)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var result = await _scheduleService.ChangeManagerMasterSchedule(salonId.Value, scheduleId, request);
                return result.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpPost("manager/account/master/{masterId}")]
        public async Task<IActionResult> CreateManagerMasterSchedule(int masterId, [FromBody] MasterScheduleCreateDto request)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var result = await _scheduleService.SetManagerMasterSchedule(salonId.Value, masterId, request);
                return result.MakeResponse();
            }
            return salonId.MakeResponse();
        }
    }
}
