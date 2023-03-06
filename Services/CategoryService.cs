using Dapper;
using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;
        private readonly IDbConnectionManager _connectionManager;

        public CategoryService(DataContext context, IDbConnectionManager connectionManager)
        {
            _context = context;
            _connectionManager = connectionManager;
        }

        public async Task<IEnumerable<ServiceCategory>> GetAllCategories()
        {
            var query = "SELECT * FROM ServiceCategory";

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                var categories = await connection
                    .QueryAsync<ServiceCategory>(query);
                return categories.ToList();
            }
        }
    }
}