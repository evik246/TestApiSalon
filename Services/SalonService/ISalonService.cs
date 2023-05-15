using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.SalonService
{
    public interface ISalonService
    {
        Task<Result<IEnumerable<Salon>>> GetAllSalons(Paging paging);
        Task<Result<IEnumerable<Salon>>> GetSalonsInCity(int cityId, Paging paging);
    }
}
