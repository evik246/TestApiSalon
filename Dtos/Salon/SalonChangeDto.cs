using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Salon
{
    public class SalonChangeDto
    {
        public string? Address { get; set; }

        public int? CityId { get; set; }

        [Phone(ErrorMessage = "Phone number is invalid")]
        [RegularExpression(@"\+[0-9]{12}", ErrorMessage = "Phone number is invalid")]
        public string? Phone { get; set; }
    }
}
