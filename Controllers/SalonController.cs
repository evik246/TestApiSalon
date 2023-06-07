using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.SalonService;

namespace TestApiSalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalonController : ControllerBase
    {
        private readonly ISalonService _salonService;

        public SalonController(ISalonService salonService)
        {
            _salonService = salonService;
        }

        [Roles("Guest", "Client")]
        [HttpGet]
        public async Task<IActionResult> GetSalons([FromQuery] Paging paging)
        {
            var salons = await _salonService.GetAllSalons(paging);
            return salons.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("city/{id}")]
        public async Task<IActionResult> GetSalonsInCity(int id, [FromQuery] Paging paging)
        {
            var salons = await _salonService.GetSalonsInCity(id, paging);
            return salons.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalonWithAddressById(int id)
        {
            var salon = await _salonService.GetSalonWithAddressById(id);
            return salon.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account")]
        public async Task<IActionResult> GetManagerSalon()
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var salon = await _salonService.GetSalonById(salonId.Value!);
                return salon.MakeResponse();
            }
            return salonId.MakeResponse();
        }
    }
}
