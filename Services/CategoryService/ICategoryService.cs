using TestApiSalon.Dtos.Category;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<ServiceCategory>>> GetAllCategories(Paging paging);
        Task<Result<IEnumerable<ServiceCategory>>> GetCategoriesInSalon(int salonId, Paging paging);
        Task<Result<IEnumerable<ServiceCategory>>> GetMasterCategories(int masterId, Paging paging);
        Task<Result<string>> CreateCategory(CategoryDto request);
        Task<Result<string>> UpdateCategory(int categoryId, CategoryDto request);
        Task<Result<string>> DeleteCategory(int categoryId);
    }
}