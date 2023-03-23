using Dapper;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDbConnectionService _connectionService;

        public EmployeeService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            var query = "SELECT " + 
                "e.id, " +
                "e.name, " +
                "e.last_name AS LastName, " +
                "e.salon_id AS SalonId, " +
                "e.email, " +
                "e.password, " +
                "e.role, " +
                "e.photo_path AS PhotoName, " +
                "e.specialization " +
                "FROM Employee e;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employees = await connection
                    .QueryAsync<Employee>(query);
                return employees.ToList();
            }
        }
    }
}
