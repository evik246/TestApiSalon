using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<Service?> GetServiceById(int id);
    }
}