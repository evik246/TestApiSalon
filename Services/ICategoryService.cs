using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public interface ICategoryService : IDbConnectionBase
    {
        Task<IEnumerable<ServiceCategory>> GetAllCategories();
    }
}