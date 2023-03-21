using Dapper;
using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services
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
            var query = "SELECT " +
                    "s.id, " +
                    "s.name, " +
                    "s.category_id AS CategoryId, " +
                    "s.price, " +
                    "s.execution_time AS ExecutionTime, " +
                    "c.* " +
                    "FROM Service s " +
                    "JOIN ServiceCategory c ON s.category_id = c.id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection
                    .QueryAsync<Service, ServiceCategory, Service>(
                    query, (service, category) =>
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

            var query = "SELECT " + 
                    "s.id, " + 
                    "s.name, " + 
                    "s.category_id AS CategoryId, " + 
                    "s.price, " + 
                    "s.execution_time AS ExecutionTime, " +
                    "c.* " +
                    "FROM Service s " +
                    "JOIN ServiceCategory c ON s.category_id = c.id " +
                    "WHERE s.id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection
                    .QueryAsync<Service, ServiceCategory, Service>(
                    query, (service, category) =>
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