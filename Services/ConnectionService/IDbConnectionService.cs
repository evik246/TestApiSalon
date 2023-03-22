using System.Data;
using TestApiSalon.Data;

namespace TestApiSalon.Services.ConnectionService
{
    public interface IDbConnectionService
    {
        DbConnectionName ConnectionName { get; set; }
        IDbConnection CreateConnection();
    }
}
