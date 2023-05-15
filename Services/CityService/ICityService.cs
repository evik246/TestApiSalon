using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CityService
{
    public interface ICityService
    {
        Task<Result<IEnumerable<City>>> GetAllCities(Paging paging);
    }
}
