using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
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
    }
}
