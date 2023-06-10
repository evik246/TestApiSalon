using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Category;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.CategoryService;

namespace TestApiSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Roles("Guest", "Client", "Admin")]
        [HttpGet("salon/{id}")]
        public async Task<IActionResult> GetCategoriesInSalon(int id, [FromQuery] Paging paging)
        {
            var categories = await _categoryService.GetCategoriesInSalon(id, paging);
            return categories.MakeResponse();
        }

        [Roles("Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] Paging paging)
        {
            var categories = await _categoryService.GetAllCategories(paging);
            return categories.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("manager/account")]
        public async Task<IActionResult> GetManagerSalonCategories([FromQuery] Paging paging)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var categories = await _categoryService.GetCategoriesInSalon(salonId.Value, paging);
                return categories.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterCategories(int id, [FromQuery] Paging paging)
        {
            var categories = await _categoryService.GetMasterCategories(id, paging);
            return categories.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto request)
        {
            var result = await _categoryService.CreateCategory(request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeCategory(int id, [FromBody] CategoryDto request)
        {
            var result = await _categoryService.UpdateCategory(id, request);
            return result.MakeResponse();
        }

        [Roles("Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return result.MakeResponse();
        }
    }
}