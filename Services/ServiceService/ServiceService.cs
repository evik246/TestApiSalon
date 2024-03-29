using Dapper;
using Npgsql;
using System.Data;
using System.Text;
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

        public async Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetServicesInSalonByCategory(int salonId, int categoryId, Paging paging)
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

        public async Task<Result<ServiceWithoutCategoryDto>> GetServiceWithoutCategoryById(int serviceId)
        {
            var parameters = new
            {
                ServiceId = serviceId
            };

            var query = "SELECT s.* FROM Service s "
                        + "WHERE s.id = @ServiceId;";

            using (var connection = _connectionService.CreateConnection())
            {
                var service = await connection.QueryFirstOrDefaultAsync<ServiceWithoutCategoryDto>(query, parameters);

                if (service == null)
                {
                    return new Result<ServiceWithoutCategoryDto>(new NotFoundException("Service is not found"));
                }
                return new Result<ServiceWithoutCategoryDto>(service);
            }
        }

        public async Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetMasterServicesByCategoryAndSalon(int salonId, int masterId, int categoryId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                MasterId = masterId,
                CategoryId = categoryId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT s.id, s.name, s.price, s.execution_time "
                        + "FROM Service s "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "JOIN Employee e ON sk.employee_id = e.id "
                        + "WHERE sk.employee_id = @MasterId "
                        + "AND c.id = @CategoryId "
                        + "AND e.salon_id = @SalonId "
                        + "ORDER BY s.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection.QueryAsync<ServiceWithoutCategoryDto>(query, parameters);
                return new Result<IEnumerable<ServiceWithoutCategoryDto>>(services);
            }
        }

        public async Task<Result<IEnumerable<ServiceAppointmentCount>>> GetTopServices(int salonId, int top)
        {
            var parameters = new
            {
                SalonId = salonId,
                Top = top
            };

            var query = "SELECT s.id, s.name, s.price, s.execution_time, "
                        + "c.id AS category_id, c.name AS category_name, "
                        + "COUNT(*) AS appointments_count "
                        + "FROM Appointment a "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "JOIN Employee e ON e.id = a.employee_id "
                        + "WHERE e.salon_id = @SalonId "
                        //+ "AND a.status = 'Completed' "
                        + "GROUP BY s.id, s.name, s.price, s.execution_time, c.id, c.name "
                        + "ORDER BY appointments_count DESC "
                        + "LIMIT @Top;";

            using (var connection = _connectionService.CreateConnection())
            {
                var result = await connection.QueryAsync(query, parameters);

                var services = result.Select(r => new ServiceAppointmentCount
                {
                    Id = r.id,
                    Name = r.name,
                    Price = r.price,
                    ExecutionTime = r.execution_time,
                    Category = new ServiceCategory
                    {
                        Id = r.category_id,
                        Name = r.category_name
                    },
                    AppointmentsCount = r.appointments_count
                });

                return new Result<IEnumerable<ServiceAppointmentCount>>(services);
            }
        }

        public async Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetAllServicesByCategory(int categoryId, Paging paging)
        {
            var parameters = new
            {
                CategoryId = categoryId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT s.id, s.name, s.price, s.execution_time "
                        + "FROM Service s "
                        + "JOIN ServiceCategory c ON c.id = s.category_id "
                        + "WHERE c.id = @CategoryId "
                        + "ORDER BY s.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var services = await connection.QueryAsync<ServiceWithoutCategoryDto>(query, parameters);
                return new Result<IEnumerable<ServiceWithoutCategoryDto>>(services);
            }
        }

        public async Task<Result<string>> CreateService(ServiceCreateDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            parameters.Add("CategoryId", request.CategoryId, DbType.Int32);
            parameters.Add("Price", request.Price, DbType.Decimal);
            parameters.Add("ExecutionTime", request.ExecutionTime, DbType.Time);

            var query = "INSERT INTO Service (name, category_id, price, execution_time) " 
                        + "VALUES (@Name, @CategoryId, @Price, @ExecutionTime);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.QueryAsync(query, parameters);
                    return new Result<string>("Service is created successfully");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001"))
                {
                    return new Result<string>(new ConflictException(ex.MessageText));
                }
            }
        }

        public async Task<Result<string>> UpdateService(int serviceId, ServiceChangeDto request)
        {
            var query = new StringBuilder("UPDATE Service SET ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query.Append("name = @Name, ");
                parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            }

            if (request.CategoryId != null)
            {
                query.Append("category_id = @CategoryId, ");
                parameters.Add("CategoryId", request.CategoryId, DbType.Int32);
            }

            if (request.Price != null)
            {
                query.Append("price = @Price, ");
                parameters.Add("Price", request.Price, DbType.Decimal);
            }

            if (request.ExecutionTime != null)
            {
                query.Append("execution_time = @ExecutionTime, ");
                parameters.Add("ExecutionTime", request.ExecutionTime, DbType.Time);
            }

            if (query.ToString().EndsWith(", "))
            {
                query.Remove(query.Length - 2, 2);
            }

            query.Append(" WHERE id = @Id;");
            parameters.Add("Id", serviceId, DbType.Int32);

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query.ToString(), parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("Service is not found"));
                }
                return new Result<string>("Service is changed");
            }
        }

        public async Task<Result<string>> DeleteService(int serviceId)
        {
            var parameters = new
            {
                ServiceId = serviceId
            };

            var query = "DELETE FROM Service WHERE id = @ServiceId;";

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("Service is not found"));
                }
                return new Result<string>("Service is deleted");
            }
        }
    }
}