namespace TestApiSalon.Dtos
{
    public class ServiceForCreateDto
    {
        public required string Name { get; set; }

        public double Price { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public int CategoryId { get; set; }
    }
}