using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Salon;
using TestApiSalon.Models;

namespace TestApiSalon.Services.SalonService
{
    public interface ISalonService
    {
        Task<Result<IEnumerable<Salon>>> GetAllSalons(Paging paging);
        Task<Result<IEnumerable<Salon>>> GetSalonsInCity(int cityId, Paging paging);
        Task<Result<SalonWithAddress>> GetSalonWithAddressById(int salonId);
    }
}
