using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Cities;
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

        [Roles("Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityDto request)
        {
            var result = await _cityService.CreateCity(request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeCity(int id, [FromBody] CityDto request)
        {
            var result = await _cityService.UpdateCity(id, request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var result = await _cityService.DeleteCity(id);
            return result.MakeResponse();
        }
    }
}
