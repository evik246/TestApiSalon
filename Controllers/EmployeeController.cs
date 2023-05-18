using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
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

        [Roles("Guest", "Client")]
        [HttpGet("master/{id}/service")]
        public async Task<IActionResult> GetMasterWithServicesById(int id)
        {
            var master = await _employeeService.GetMasterWithServices(id);
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

        [Roles("Guest", "Client")]
        [HttpGet("salon/{id}/master")]
        public async Task<IActionResult> GetAllMastersWithServices(int id, [FromQuery] Paging paging)
        {
            var master = await _employeeService.GetAllMastersWithServices(id, paging);
            return master.MakeResponse();
        }
    }
}
