using Dapper;
using TestApiSalon.Dtos.Category;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IDbConnectionService _connectionService;

        public CategoryService(IDbConnectionService connectionManager)
        {
            _connectionService = connectionManager;
        }

        public async Task<Result<string>> CreateCategory(CategoryDto request)
        {
            var parameters = new
            {
                Name = request.Name
            };

            var query = "INSERT INTO ServiceCategory(name) VALUES (@Name);";

            using (var connection = _connectionService.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return new Result<string>("Service category is created");
            }
        }

        public async Task<Result<string>> UpdateCategory(int categoryId, CategoryDto request)
        {
            var parameters = new
            {
                CategoryId = categoryId,
                Name = request.Name
            };

            var query = "UPDATE ServiceCategory SET name = @Name WHERE id = @CategoryId;";

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("Service category is not found"));
                }
                return new Result<string>("Service category is changed");
            }
        }

        public async Task<Result<string>> DeleteCategory(int categoryId)
        {
            var parameters = new
            {
                CategoryId = categoryId
            };

            var query = "DELETE FROM ServiceCategory WHERE id = @CategoryId;";

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("Service category is not found"));
                }
                return new Result<string>("Service category is deleted");
            }
        }

        public async Task<Result<IEnumerable<ServiceCategory>>> GetAllCategories(Paging paging)
        {
            var parameters = new 
            {
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT * FROM ServiceCategory OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var categories = await connection
                    .QueryAsync<ServiceCategory>(query, parameters);
                return new Result<IEnumerable<ServiceCategory>>(categories);
            }
        }

        public async Task<Result<IEnumerable<ServiceCategory>>> GetCategoriesInSalon(int salonId, Paging paging)
        {
            var parameters = new
            {
                Id = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT DISTINCT c.id, c.name "
                        + "FROM ServiceCategory c "
                        + "JOIN Service s ON s.category_id = c.id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "JOIN Employee e ON sk.employee_id = e.id "
                        + "JOIN Salon sa ON e.salon_id = sa.id "
                        + "WHERE sa.id = @Id "
                        + "ORDER BY c.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var categories = await connection.QueryAsync<ServiceCategory>(query, parameters);
                return new Result<IEnumerable<ServiceCategory>>(categories);
            }
        }

        public async Task<Result<IEnumerable<ServiceCategory>>> GetMasterCategories(int masterId, Paging paging)
        {
            var parameters = new
            {
                MasterId = masterId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT DISTINCT c.id, c.name "
                        + "FROM ServiceCategory c "
                        + "JOIN Service s ON s.category_id = c.id "
                        + "JOIN Skill sk ON sk.service_id = s.id "
                        + "WHERE sk.employee_id = @MasterId "
                        + "ORDER BY c.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var categories = await connection.QueryAsync<ServiceCategory>(query, parameters);
                return new Result<IEnumerable<ServiceCategory>>(categories);
            }
        }
    }
}