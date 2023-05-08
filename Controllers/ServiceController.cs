using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
using TestApiSalon.Extensions;
using TestApiSalon.Services.ServiceService;

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
        public async Task<IActionResult> GetServices([FromQuery] Paging paging)
        {
            var services = await _serviceService.GetAllServices(paging);
            return services.MakeResponse();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceService.GetServiceById(id);
            return service.MakeResponse();
        }
    }
}