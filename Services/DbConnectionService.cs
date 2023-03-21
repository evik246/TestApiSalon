using System.Data;
using TestApiSalon.Data;

namespace TestApiSalon.Services
{
    public class DbConnectionService : IDbConnectionService
    {
        private readonly DataContext _context;

        public DbConnectionName ConnectionName { get; set; }

        public DbConnectionService(DataContext context)
        {
            ConnectionName = DbConnectionName.Guest;
            _context = context;
        }

        public IDbConnection CreateConnection()
        {
            var connection = _context.CreateConnection(ConnectionName);
            connection.Open();
            return connection;
        }
    }
}
