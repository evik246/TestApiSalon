using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Salon
{
    public class SalonCreateDto
    {
        [Required(ErrorMessage = "Salon address is required")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Salon city is required")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Phone number is invalid")]
        [RegularExpression(@"\+[0-9]{12}", ErrorMessage = "Phone number is invalid")]
        public required string Phone { get; set; }
    }
}
