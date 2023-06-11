using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos.Salon
{
    [AtLeastOneProperty(ErrorMessage = "At least one property must be specified")]
    public class SalonChangeDto
    {
        public string? Address { get; set; }

        public int? CityId { get; set; }

        [Phone(ErrorMessage = "Phone number is invalid")]
        [RegularExpression(@"\+[0-9]{12}", ErrorMessage = "Phone number is invalid")]
        public string? Phone { get; set; }
    }
}
