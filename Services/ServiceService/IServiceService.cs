using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ServiceService
{
    public interface IServiceService
    {
        Task<Result<IEnumerable<Service>>> GetAllServices();
        Task<Result<Service>> GetServiceById(int id);
    }
}