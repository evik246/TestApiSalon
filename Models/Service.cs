using System.Text.Json.Serialization;

namespace TestApiSalon.Models
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Price { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        [JsonIgnore]
        public int CategoryId { get; set; }

        public ServiceCategory Category { get; set; } = new ServiceCategory();
    }
}