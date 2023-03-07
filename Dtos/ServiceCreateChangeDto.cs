using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class ServiceCreateChangeDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [Range(1, 999999, ErrorMessage = "Invalid service price")]
        public double Price { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "00:00", "23:59", ErrorMessage = "Invalid service execution time")]
        public TimeSpan ExecutionTime { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}