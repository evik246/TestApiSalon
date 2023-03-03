using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Models;
using TestApiSalon.Services;

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
        public async Task<ActionResult<IEnumerable<ServiceCategory>>> GetCategories()
        {
            try
            {
                _categoryService.ConnectionName = Data.DbConnectionName.Client;
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}