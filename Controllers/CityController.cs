using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetCities([FromQuery] Paging paging)
        {
            var cities = await _cityService.GetAllCities(paging);
            return cities.MakeResponse();
        }
    }
}
