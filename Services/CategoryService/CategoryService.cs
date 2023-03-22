using Dapper;
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

        public async Task<IEnumerable<ServiceCategory>> GetAllCategories()
        {
            var query = "SELECT * FROM ServiceCategory";

            using (var connection = _connectionService.CreateConnection())
            {
                var categories = await connection
                    .QueryAsync<ServiceCategory>(query);
                return categories.ToList();
            }
        }
    }
}