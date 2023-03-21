using System.Data;
using TestApiSalon.Data;

namespace TestApiSalon.Services
{
    public interface IDbConnectionService
    {
        DbConnectionName ConnectionName { get; set; }
        IDbConnection CreateConnection();
    }
}
