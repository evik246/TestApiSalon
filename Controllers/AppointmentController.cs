using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;
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

        [Roles("Client")]
        [HttpGet("customer/account/salon/{salonId}")]
        public async Task<IActionResult> GetCustomerAppointmentsInAccount(int salonId, [FromQuery] Paging paging)
        {
            var customerId = this.GetAuthorizedUserId();
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
            var customerId = this.GetAuthorizedUserId();
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
            var customerId = this.GetAuthorizedUserId();
            if (customerId.State == ResultState.Success)
            {
                var result = await _appointmentService.CancelAppointment(customerId.Value, appointmentId);
                return result.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Client")]
        [HttpGet("{appointmentId}/customer/account")]
        public async Task<IActionResult> GetCustomerAppointmentById(int appointmentId)
        {
            var customerId = this.GetAuthorizedUserId();
            if (customerId.State == ResultState.Success)
            {
                var result = await _appointmentService.GetCustomerAppointmentById(customerId.Value, appointmentId);
                return result.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account")]
        public async Task<IActionResult> GetMasterAppointments([FromQuery] Paging paging)
        {
            var employeeId = this.GetAuthorizedUserId();
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
            var employeeId = this.GetAuthorizedUserId();
            if (employeeId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetMasterAppintments(employeeId.Value, paging, customerId);
                return appointments.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Master")]
        [HttpPut("{id}/master/account/mark_complete")]
        public async Task<IActionResult> MarkAppointmentComplete(int id)
        {
            var masterId = this.GetAuthorizedUserId();
            if (masterId.State == ResultState.Success)
            {
                var result = await _appointmentService.MarkMasterAppointmentCompleted(masterId.Value, id);
                return result.MakeResponse();
            }
            return masterId.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account/count")]
        public async Task<IActionResult> GetCompletedAppointmentsCount([FromQuery] DateRangeDto dateRange)
        {
            var employeeId = this.GetAuthorizedUserId();
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
            var employeeId = this.GetAuthorizedUserId();
            if (employeeId.State == ResultState.Success)
            {
                var income = await _appointmentService.GetCompletedAppointmentsIncome(employeeId.Value, dateRange);
                return income.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account")]
        public async Task<IActionResult> GetManagerAppointments([FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetManagerAppointments(salonId.Value!, paging);
                return appointments.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("active/manager/account")]
        public async Task<IActionResult> GetManagerActiveAppointments([FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetManagerActiveAppointments(salonId.Value!, paging);
                return appointments.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("completed/manager/account")]
        public async Task<IActionResult> GetManagerCompletedAppointments([FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetManagerCompletedAppointments(salonId.Value!, paging);
                return appointments.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("uncompleted/manager/account")]
        public async Task<IActionResult> GetManagerUncompletedAppointments([FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var appointments = await _appointmentService.GetManagerUncompletedAppointments(salonId.Value!, paging);
                return appointments.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpPut("{appointmentId}/manager/account/mark_complete")]
        public async Task<IActionResult> MarkManagerAppointmentCompleted(int appointmentId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var result = await _appointmentService.MarkManagerAppointmentCompleted(salonId.Value!, appointmentId);
                return result.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpPut("{appointmentId}/master/{masterId}/change/manager/account")]
        public async Task<IActionResult> ChangeAppointmentMaster(int appointmentId, int masterId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var result = await _appointmentService.ChangeMasterInAppointment(salonId.Value!, appointmentId, masterId);
                return result.MakeResponse();
            }
            return salonId.MakeResponse();
        }
    }
}
