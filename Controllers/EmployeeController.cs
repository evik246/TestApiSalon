using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
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
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployees();
            return employees.MakeResponse();
        }

        [HttpGet("{id}/photo")]
        public async Task<IActionResult> GetEmployeePhoto(int id)
        {
            var file = await _employeeService.GetEmployeePhoto(id);
            return file.Match(stream =>
            {
                return File(stream, MediaTypeNames.Image.Jpeg);
            }, exception =>
            {
                return file.MakeResponse();
            });
        }
    }
}
