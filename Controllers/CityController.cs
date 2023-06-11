using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.CityService;

namespace TestApiSalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCities([FromQuery] Paging paging)
        {
            var cities = await _cityService.GetAllCities(paging);
            return cities.MakeResponse();
        }
    }
}
