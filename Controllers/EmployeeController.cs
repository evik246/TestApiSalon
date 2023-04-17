using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
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
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployees();
            return Ok(employees);
        }

        [HttpGet("{id}/photo")]
        public async Task<ActionResult> GetEmployeePhoto(int id)
        {
            var file = await _employeeService.GetEmployeePhoto(id) 
                ?? throw new NotFoundException("Photo of the employee is not found");
            return File(file, MediaTypeNames.Image.Jpeg);
        }
    }
}
