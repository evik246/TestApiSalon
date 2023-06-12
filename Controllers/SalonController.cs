using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Salon;
using TestApiSalon.Dtos.Schedule;
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

        [Roles("Guest", "Client", "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetSalons([FromQuery] Paging paging)
        {
            var salons = await _salonService.GetAllSalons(paging);
            return salons.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("city/{id}")]
        public async Task<IActionResult> GetSalonsInCity(int id, [FromQuery] Paging paging)
        {
            var salons = await _salonService.GetSalonsInCity(id, paging);
            return salons.MakeResponse();
        }

        [Roles("Guest", "Client", "Admin")]
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

        [Roles("Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSalon([FromBody] SalonCreateDto request)
        {
            var result = await _salonService.CreateSalon(request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeSalon(int id, [FromBody] SalonChangeDto request)
        {
            var result = await _salonService.UpdateSalon(id, request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalon(int id)
        {
            var result = await _salonService.DeleteSalon(id);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpGet("{salonId}/income")]
        public async Task<IActionResult> GetSalonIncome(int salonId, [FromQuery] DateRangeDto dateRange)
        {
            var income = await _salonService.GetSalonIncome(salonId, dateRange);
            return income.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account/income")]
        public async Task<IActionResult> GetSalonIncome([FromQuery] DateRangeDto dateRange)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var income = await _salonService.GetSalonIncome(salonId.Value!, dateRange);
                return income.MakeResponse();
            }
            return salonId.MakeResponse();
        }
    }
}
