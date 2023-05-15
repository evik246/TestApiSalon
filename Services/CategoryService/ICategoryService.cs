using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<ServiceCategory>>> GetAllCategories(Paging paging);
        Task<Result<IEnumerable<ServiceCategory>>> GetCategoriesInSalon(int salonId, Paging paging);
    }
}