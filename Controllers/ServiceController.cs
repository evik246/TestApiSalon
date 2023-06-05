using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Other;
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

        [Roles("Guest", "Client")]
        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterServices(int id, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetMasterServices(id, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("salon/{id}")]
        public async Task<IActionResult> GetSalonServices(int id, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetServicesInSalon(id, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("salon/{salonId}/category/{categoryId}")]
        public async Task<IActionResult> GetSalonServicesByCategory(int salonId, int categoryId, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetServicesByCategory(salonId, categoryId, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("master/{masterId}/category/{categoryId}")]
        public async Task<IActionResult> GetMasterServicesByCategory(int masterId, int categoryId, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetMasterServicesByCategory(masterId, categoryId, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceService.GetServiceWithoutCategoryById(id);
            return service.MakeResponse();
        }
    }
}