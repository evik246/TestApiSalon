using System.Text.Json.Serialization;

namespace TestApiSalon.Models
{
    public class Salon
    {
        public int Id { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [JsonIgnore]
        public int CityId { get; set; }

        public City? City { get; set; }
    }
}
