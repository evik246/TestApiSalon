using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ServiceService
{
    public interface IServiceService
    {
        Task<Result<IEnumerable<Service>>> GetAllServices(Paging paging);
        Task<Result<Service>> GetServiceById(int id);
        Task<Result<IEnumerable<Service>>> GetMasterServices(int masterId, Paging paging);
        Task<Result<IEnumerable<Service>>> GetServicesInSalon(int salonId, Paging paging);
    }
}