using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetSalons([FromQuery] Paging paging)
        {
            var salons = await _salonService.GetAllSalons(paging);
            return salons.MakeResponse();
        }

        [HttpGet("city/{id}")]
        public async Task<IActionResult> GetSalonsInCity(int id, [FromQuery] Paging paging)
        {
            var salons = await _salonService.GetSalonsInCity(id, paging);
            return salons.MakeResponse();
        }
    }
}
