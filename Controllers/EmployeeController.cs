using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestApiSalon.Dtos;
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

        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] Paging paging)
        {
            var employees = await _employeeService.GetAllEmployees(paging);
            return employees.MakeResponse();
        }

        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetEmployeePhoto(int id)
        {
            var file = await _employeeService.GetEmployeePhoto(id);
            return file.MakeFileResponse(this);
        }

        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterWithServicesById(int id)
        {
            var master = await _employeeService.GetMasterWithServices(id);
            return master.MakeResponse();
        }

        [HttpGet("salon/{id}/master")]
        public async Task<IActionResult> GetAllMastersWithServices(int id, [FromQuery] Paging paging)
        {
            var master = await _employeeService.GetAllMastersWithServices(id, paging);
            return master.MakeResponse();
        }
    }
}
