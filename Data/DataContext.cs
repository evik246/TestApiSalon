using System.Data;
using Npgsql;

namespace TestApiSalon.Data
{
    public class DataContext : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IDictionary<DbConnectionName, string> _connections;

        public DataContext(IConfiguration configuration, IDictionary<DbConnectionName, string> connections)
        {
            _configuration = configuration;
            _connections = connections;
        }

        public IDbConnection CreateConnection(DbConnectionName connectionName)
        {
            if (_connections.TryGetValue(connectionName, out string? connectionString))
            {
                return new NpgsqlConnection(_configuration.GetConnectionString(connectionString));
            }
            throw new ArgumentNullException();
        }
    }
}