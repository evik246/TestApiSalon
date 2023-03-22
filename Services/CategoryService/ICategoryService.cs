using TestApiSalon.Models;

namespace TestApiSalon.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<IEnumerable<ServiceCategory>> GetAllCategories();
    }
}