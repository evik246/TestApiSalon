using Dapper;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
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

        public async Task<Result<IEnumerable<Service>>> GetAllServices(Paging paging)
        {
            var parameters = new 
            {
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT s.*, c.* FROM Service s " +
                "JOIN ServiceCategory c ON s.category_id = c.id " +
                "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection
                    .QueryAsync(
                    query, (Service service, ServiceCategory category) =>
                    {
                        service.Category = category;
                        return service;
                    }, param: parameters
                );
                return new Result<IEnumerable<Service>>(services);
            }
        }

        public async Task<Result<Service>> GetServiceById(int id)
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
                    return new Result<Service>(new NotFoundException("Service is not found"));
                }
                return new Result<Service>(services.First());
            }
        }
    }
}