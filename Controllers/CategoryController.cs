using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
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

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] Paging paging)
        {
            var categories = await _categoryService.GetAllCategories(paging);
            return categories.MakeResponse();
        }
    }
}