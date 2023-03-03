using System.Data;

namespace TestApiSalon.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(DbConnectionName connectionName);
    }
}
