using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Cities
{
    public class CityDto
    {
        [Required(ErrorMessage = "City name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
