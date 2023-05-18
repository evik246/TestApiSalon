﻿using Microsoft.AspNetCore.Mvc;
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

        [Roles("Client")]
        [HttpGet("master/{id}/working_days")]
        public async Task<IActionResult> GetMasterWorkingDays(int id, [FromQuery] DateRangeDto dateRange)
        {
            var days = await _scheduleService.GetMasterWorkingDays(id, dateRange);
            return days.MakeResponse();
        }
    }
}
