using TestApiSalon.Dtos.Cities;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CityService
{
    public interface ICityService
    {
        Task<Result<IEnumerable<City>>> GetAllCities(Paging paging);
        Task<Result<string>> CreateCity(CityDto request);
        Task<Result<string>> UpdateCity(int cityId, CityDto request);
        Task<Result<string>> DeleteCity(int cityId);
    }
}
