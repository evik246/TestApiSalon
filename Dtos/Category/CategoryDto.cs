using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Category
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Service category name is required")]
        public required string Name { get; set; }
    }
}
