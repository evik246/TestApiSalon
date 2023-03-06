using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<ServiceCategory>> GetAllCategories();
    }
}