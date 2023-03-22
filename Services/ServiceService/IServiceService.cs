using TestApiSalon.Models;

namespace TestApiSalon.Services.ServiceService
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<Service?> GetServiceById(int id);
    }
}