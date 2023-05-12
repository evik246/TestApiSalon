using System.Text.Json.Serialization;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Salon;

namespace TestApiSalon.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public DateTime PublishedDate { get; set; }

        public string? Review { get; set; }

        public int Rating { get; set; }

        [JsonIgnore]
        public int SalonId { get; set; }

        public SalonDto? Salon { get; set; }

        [JsonIgnore]
        public int CustomerId { get; set; }

        public CustomerDto? Customer { get; set; }
    }
}
