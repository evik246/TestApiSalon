using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.EmployeeService;

namespace TestApiSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetEmployeePhoto(int id)
        {
            var file = await _employeeService.GetEmployeePhoto(id);
            return file.MakeFileResponse(this);
        }

        [Roles("Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] Paging paging, [FromQuery] EmployeeFiltrationDto filtration)
        {
            var employees = await _employeeService.GetAllEmployees(paging, filtration);
            return employees.MakeResponse();
        }

        [Roles("Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            return employee.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto request)
        {
            var result = await _employeeService.CreateEmployee(request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> ChangeEmployee(int employeeId, [FromBody] EmployeeChangeDto request)
        {
            var result = await _employeeService.UpdateEmployee(employeeId, request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var result = await _employeeService.DeleteEmployee(employeeId);
            return result.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("master/{id}/service")]
        public async Task<IActionResult> GetMasterWithServicesById(int id)
        {
            var master = await _employeeService.GetMasterWithServices(id);
            return master.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("salon/{salonId}/master/service/{serviceId}")]
        public async Task<IActionResult> GetMastersByService(int salonId, int serviceId, [FromQuery] Paging paging)
        {
            var masters = await _employeeService.GetMastersWithNameByService(salonId, serviceId, paging);
            return masters.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterById(int id)
        {
            var master = await _employeeService.GetMasterById(id);
            return master.MakeResponse();
        }

        [Roles("Master")]
        [HttpGet("master/account")]
        public async Task<IActionResult> GetMaster()
        {
            var employeeId = this.GetAuthorizedUserId();
            if (employeeId.State == ResultState.Success)
            {
                var master = await _employeeService.GetMaster(employeeId.Value);
                return master.MakeResponse();
            }
            return employeeId.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("salon/{id}/master")]
        public async Task<IActionResult> GetAllMastersWithServices(int id, [FromQuery] Paging paging)
        {
            var master = await _employeeService.GetAllMastersWithServices(id, paging);
            return master.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/master")]
        public async Task<IActionResult> GetManagerMasters([FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var masters = await _employeeService.GetManagerMasters(salonId.Value, paging);
                return masters.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/master/service/{serviceId}")]
        public async Task<IActionResult> GetManagerMastersByService(int serviceId, [FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var masters = await _employeeService.GetManagerMastersByService(salonId.Value, serviceId, paging);
                return masters.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/master/category/{categoryId}")]
        public async Task<IActionResult> GetManagerMastersByServiceCategory(int categoryId, [FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var masters = await _employeeService.GetManagerMastersByCategory(salonId.Value, categoryId, paging);
                return masters.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/master/{masterId}")]
        public async Task<IActionResult> GetManagerMasterById(int masterId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var master = await _employeeService.GetManagerMasterById(salonId.Value, masterId);
                return master.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("appointment/{appointmentId}/available/manager/account")]
        public async Task<IActionResult> GetAvailableMasters(int appointmentId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var masters = await _employeeService.GetAvailableMastersToChangeAnother(salonId.Value!, appointmentId);
                return masters.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("top/{top}/manager/account")]
        public async Task<IActionResult> GetTopMasters(int top)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var masters = await _employeeService.GetTopMasters(salonId.Value!, top);
                return masters.MakeResponse();
            }
            return salonId.MakeResponse();
        }
    }
}
