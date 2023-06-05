using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Salon
{
    public class SalonWithAddress
    {
        public int Id { get; set; }

        public string Address { get; set; } = string.Empty;

        public City City { get; set; } = new();

        public override string ToString()
        {
            return City.Name + ", " + Address;
        }
    }
}
