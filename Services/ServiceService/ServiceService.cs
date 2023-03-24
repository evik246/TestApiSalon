using Dapper;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.ServiceService
{
    public class ServiceService : IServiceService
    {
        private readonly IDbConnectionService _connectionService;

        public ServiceService(IDbConnectionService connectionManager)
        {
            _connectionService = connectionManager;
        }

        public async Task<IEnumerable<Service>> GetAllServices()
        {
            var query = "SELECT s.*, c.* FROM Service s " +
                "JOIN ServiceCategory c ON s.category_id = c.id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection
                    .QueryAsync(
                    query, (Service service, ServiceCategory category) =>
                    {
                        service.Category = category;
                        return service;
                    }
                );
                return services.ToList();
            }
        }

        public async Task<Service?> GetServiceById(int id)
        {
            var parameters = new
            {
                Id = id
            };

            var query = "SELECT s.*, c.* FROM Service s " +
                "JOIN ServiceCategory c ON s.category_id = c.id " +
                "WHERE s.id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection
                    .QueryAsync(
                    query, (Service service, ServiceCategory category) =>
                    {
                        service.Category = category;
                        return service;
                    },
                    parameters
                );
                if (!services.Any())
                {
                    return null;
                }
                return services.First();
            }
        }
    }
}