using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiSalon.Models
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Price { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public int CategoryId { get; set; }

        public ServiceCategory Category { get; set; } = new ServiceCategory();
    }
}