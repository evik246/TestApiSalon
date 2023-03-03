using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public interface IServiceService : IDbConnectionBase
    {
        Task<IEnumerable<Service>> GetAllServices();
        Task<Service> GetServiceById(int id);
    }
}