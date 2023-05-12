using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class DateRangeDto
    {
        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateOnly EndDate { get; set; }
    }
}
