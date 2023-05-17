using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;
using TestApiSalon.Extensions;
using TestApiSalon.Services.AppointmentService;
using TestApiSalon.Services.CustomerService;
using TestApiSalon.Services.EmployeeService;

namespace TestApiSalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;

        public AppointmentController(IAppointmentService appointmentService,
            ICustomerService customerService,
            IEmployeeService employeeService)
        {
            _appointmentService = appointmentService;
            _customerService = customerService;
            _employeeService = employeeService;
        }

        [Roles("Client")]
        [HttpGet("customer/account/salon/{salonId}")]
        public async Task<IActionResult> GetCustomerAppointmentsInAccount(int salonId, [FromQuery] Paging paging)
        {
            var customerId = await this.GetAuthorizedCustomerId(_customerService);
            if (customerId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetCustomerAppointments(customerId.Value, salonId, paging);
                return appointments.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Client")]
        [HttpPost("customer/account")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto request)
        {
            var customerId = await this.GetAuthorizedCustomerId(_customerService);
            if (customerId.State == ResultState.Success)
            {
                var result = await _appointmentService.CreateAppointment(customerId.Value, request);
                return result.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Client")]
        [HttpDelete("{appointmentId}/customer/account")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var customerId = await this.GetAuthorizedCustomerId(_customerService);
            if (customerId.State == ResultState.Success)
            {
                var result = await _appointmentService.CancelAppointment(customerId.Value, appointmentId);
                return result.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account")]
        public async Task<IActionResult> GetMasterAppointments([FromQuery] Paging paging)
        {
            var employeeId = await this.GetAuthorizedEmployeeId(_employeeService);
            if (employeeId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetMasterAppintments(employeeId.Value, paging);
                return appointments.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account/customer/{customerId}")]
        public async Task<IActionResult> GetMasterAppointments(int customerId, [FromQuery] Paging paging)
        {
            var employeeId = await this.GetAuthorizedEmployeeId(_employeeService);
            if (employeeId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetMasterAppintments(employeeId.Value, paging, customerId);
                return appointments.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Master")]
        [HttpPut("{id}/mark_complete")]
        public async Task<IActionResult> MarkAppointmentComplete(int id)
        {
            var result = await _appointmentService.MarkAppointmentCompleted(id);
            return result.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account/count")]
        public async Task<IActionResult> GetCompletedAppointmentsCount([FromQuery] DateRangeDto dateRange)
        {
            var employeeId = await this.GetAuthorizedEmployeeId(_employeeService);
            if (employeeId.State == ResultState.Success)
            {
                var count = await _appointmentService.GetCompletedAppointmentsCount(employeeId.Value, dateRange);
                return count.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account/income")]
        public async Task<IActionResult> GetCompletedAppointmentsIncome([FromQuery] DateRangeDto dateRange)
        {
            var employeeId = await this.GetAuthorizedEmployeeId(_employeeService);
            if (employeeId.State == ResultState.Success)
            {
                var income = await _appointmentService.GetCompletedAppointmentsIncome(employeeId.Value, dateRange);
                return income.MakeResponse();
            }
            return employeeId.MakeResponse();
        }
    }
}
