using Dapper;
using Npgsql;
using System.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IDbConnectionService _connectionService;

        public CustomerService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Customer?> CreateCustomer(CustomerRegisterDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            parameters.Add("Password", request.Password, DbType.AnsiStringFixedLength);
            parameters.Add("Phone", request.Phone, DbType.AnsiStringFixedLength);
            parameters.Add("Birthday", request.Birthday, DbType.Date);

            var query = "SELECT * FROM register_customer(@Name, @Email, @Password, @Phone, @Birthday);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    return await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    return null;
                }
            }
        }

        public async Task<Customer?> GetCustomerByEmail(string email)
        {
            var parameters = new { Email = email };

            var query = "SELECT * FROM Customer WHERE email = @Email;";

            using (var connection = _connectionService.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
            }
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            var parameters = new { Id = id };

            var query = "SELECT * FROM Customer WHERE id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
            }
        }

        public Task<Customer?> UpdateCustomer(int id, CustomerUpdateDto request)
        {
            throw new NotImplementedException();
        }
    }
}
