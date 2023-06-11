using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos.Service
{
    [AtLeastOneProperty(ErrorMessage = "At least one property must be specified")]
    public class ServiceChangeDto
    {
        [StringLength(100, ErrorMessage = "Name should be less than or equal 100 characters")]
        public string? Name { get; set; }

        [Range(0.01, 9999.99, ErrorMessage = "Price is invalid")]
        public decimal? Price { get; set; }

        [Range(typeof(TimeSpan), "00:00:01", "23:59:59", ErrorMessage = "Execution time should be less than 24 hours")]
        public TimeSpan? ExecutionTime { get; set; }

        public int? CategoryId { get; set; }
    }
}
