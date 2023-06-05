using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
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

        [Roles("Guest", "Client")]
        [HttpGet("salon/{id}")]
        public async Task<IActionResult> GetCategoriesInSalon(int id, [FromQuery] Paging paging)
        {
            var categories = await _categoryService.GetCategoriesInSalon(id, paging);
            return categories.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("master/{id}")]
        public async Task<IActionResult> GetMasterCategories(int id, [FromQuery] Paging paging)
        {
            var categories = await _categoryService.GetMasterCategories(id, paging);
            return categories.MakeResponse();
        }
    }
}