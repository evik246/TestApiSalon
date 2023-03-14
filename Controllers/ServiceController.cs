using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services;

namespace TestApiSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _serviceService.GetAllServices();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetServiceById(int id)
        {
            var service = await _serviceService.GetServiceById(id) ?? throw new NotFoundException("Service is not found");
            return Ok(service);
        }
    }
}