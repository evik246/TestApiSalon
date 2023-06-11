using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Service
{
    public class ServiceCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name should be less than or equal 100 characters")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 9999.99, ErrorMessage = "Price is invalid")]
        public required decimal Price { get; set; }

        [Required(ErrorMessage = "Execution time is required")]
        [Range(typeof(TimeSpan), "00:00:01", "23:59:59", ErrorMessage = "Execution time should be less than 24 hours")]
        public required TimeSpan ExecutionTime { get; set; }

        [Required(ErrorMessage = "Category id is required")]
        public required int CategoryId { get; set; }
    }
}