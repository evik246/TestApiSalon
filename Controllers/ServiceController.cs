using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Service;
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

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterServices(int id, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetMasterServices(id, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("salon/{id}")]
        public async Task<IActionResult> GetSalonServices(int id, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetServicesInSalon(id, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("salon/{salonId}/category/{categoryId}")]
        public async Task<IActionResult> GetSalonServicesByCategory(int salonId, int categoryId, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetServicesInSalonByCategory(salonId, categoryId, paging);
            return services.MakeResponse();
        }

        [Roles("Admin")]
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetAllServicesByCategory(int categoryId, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetAllServicesByCategory(categoryId, paging);
            return services.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/category/{categoryId}")]
        public async Task<IActionResult> GetManagerServicesByCategory(int categoryId, [FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var services = await _serviceService.GetServicesInSalonByCategory(salonId.Value, categoryId, paging);
                return services.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("master/{masterId}/category/{categoryId}")]
        public async Task<IActionResult> GetMasterServicesByCategory(int masterId, int categoryId, [FromQuery] Paging paging)
        {
            var services = await _serviceService.GetMasterServicesByCategory(masterId, categoryId, paging);
            return services.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceService.GetServiceWithoutCategoryById(id);
            return service.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/master/{masterId}/category/{categoryId}")]
        public async Task<IActionResult> GetManagerMasterServicesByCategory(int masterId, int categoryId, [FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var services = await _serviceService.GetMasterServicesByCategoryAndSalon(salonId.Value, masterId, categoryId, paging);
                return services.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("top/{top}/manager/account")]
        public async Task<IActionResult> GetTopServices(int top)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var services = await _serviceService.GetTopServices(salonId.Value, top);
                return services.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllServices([FromQuery] Paging paging)
        {
            var result = await _serviceService.GetAllServices(paging);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ServiceCreateDto request)
        {
            var result = await _serviceService.CreateService(request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeService(int id, [FromBody] ServiceChangeDto request)
        {
            var result = await _serviceService.UpdateService(id, request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var result = await _serviceService.DeleteService(id);
            return result.MakeResponse();
        }
    }
}