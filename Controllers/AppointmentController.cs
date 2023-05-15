using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.AppointmentService;

namespace TestApiSalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("salon/{salonId}/customer/{customerId}")]
        public async Task<IActionResult> GetCustomerAppointments(int customerId, int salonId, [FromQuery] Paging paging)
        {
            var appointments = await _appointmentService.GetCustomerAppointments(customerId, salonId, paging);
            return appointments.MakeResponse();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto request)
        {
            var result = await _appointmentService.CreateAppointment(request);
            return result.MakeResponse();
        }

        [HttpDelete("{appointmentId}/customer/{customerId}")]
        public async Task<IActionResult> CancelAppointment(int customerId, int appointmentId)
        {
            var result = await _appointmentService.CancelAppointment(customerId, appointmentId);
            return result.MakeResponse();
        }

        [HttpGet("master/{masterId}")]
        public async Task<IActionResult> GetMasterAppointments(int masterId, [FromQuery] Paging paging)
        {
            var appointments = await _appointmentService.GetMasterAppintments(masterId, paging);
            return appointments.MakeResponse();
        }

        [HttpGet("master/{masterId}/customer/{customerId}")]
        public async Task<IActionResult> GetMasterAppointments(int masterId, int customerId, [FromQuery] Paging paging)
        {
            var appointments = await _appointmentService.GetMasterAppintments(masterId, paging, customerId);
            return appointments.MakeResponse();
        }

        [HttpPut("{id}/mark_complete")]
        public async Task<IActionResult> MarkAppointmentComplete(int id)
        {
            var result = await _appointmentService.MarkAppointmentCompleted(id);
            return result.MakeResponse();
        }
    }
}
