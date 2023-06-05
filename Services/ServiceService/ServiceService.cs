using Dapper;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Service;
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

        public async Task<Result<IEnumerable<Service>>> GetMasterServices(int masterId, Paging paging)
        {
            var parameters = new
            {
                Id = masterId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT s.id, s.name, s.price, s.execution_time, "
                        + "c.id, c.name "
                        + "FROM Service s "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "JOIN Employee e ON sk.employee_id = e.id "
                        + "WHERE e.id = @Id "
                        + "ORDER BY s.id "
                        + "OFFSET @Skip LIMIT @Take;";

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

            var query = "SELECT s.*, c.* FROM Service s "
                        + "JOIN ServiceCategory c ON s.category_id = c.id "
                        + "WHERE s.id = @Id;";

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

        public async Task<Result<IEnumerable<Service>>> GetServicesInSalon(int salonId, Paging paging)
        {
            var parameters = new
            {
                Id = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT DISTINCT s.id, s.name, s.price, s.execution_time, "
                        + "c.id, c.name "
                        + "FROM Service s "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "JOIN Employee e ON sk.employee_id = e.id "
                        + "JOIN Salon sa ON e.salon_id = sa.id "
                        + "WHERE sa.id = @Id "
                        + "ORDER BY s.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection.QueryAsync(
                    query, (Service service, ServiceCategory category) =>
                    {
                        service.Category = category;
                        return service;
                    },
                    param: parameters
                );
                return new Result<IEnumerable<Service>>(services);
            }
        }

        public async Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetServicesByCategory(int salonId, int categoryId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                CategoryId = categoryId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT DISTINCT s.id, s.name, s.price, s.execution_time "
                        + "FROM Service s "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "JOIN Employee e ON sk.employee_id = e.id "
                        + "JOIN Salon sa ON e.salon_id = sa.id "
                        + "WHERE sa.id = @SalonId "
                        + "AND c.id = @CategoryId "
                        + "ORDER BY s.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection.QueryAsync<ServiceWithoutCategoryDto>(query, parameters);
                return new Result<IEnumerable<ServiceWithoutCategoryDto>>(services);
            }
        }

        public async Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetMasterServicesByCategory(int masterId, int categoryId, Paging paging)
        {
            var parameters = new
            {
                MasterId = masterId,
                CategoryId = categoryId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT DISTINCT s.id, s.name, s.price, s.execution_time "
                        + "FROM Service s "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "WHERE sk.employee_id = @MasterId "
                        + "AND c.id = @CategoryId "
                        + "ORDER BY s.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection.QueryAsync<ServiceWithoutCategoryDto>(query, parameters);
                return new Result<IEnumerable<ServiceWithoutCategoryDto>>(services);
            }
        }
    }
}