using Dapper;
using TestApiSalon.Data;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;

        public DbConnectionName ConnectionName { get; set; }

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceCategory>> GetAllCategories()
        {
            var query = "SELECT * FROM ServiceCategory";

            using (var connection = _context.CreateConnection(ConnectionName))
            {
                var categories = await connection
                    .QueryAsync<ServiceCategory>(query);
                return categories.ToList();
            }
        }
    }
}