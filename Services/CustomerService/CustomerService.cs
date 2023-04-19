using Dapper;
using Npgsql;
using System.Data;
using System.Text;
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

        public async Task<Customer?> UpdateCustomer(int id, CustomerUpdateDto request)
        {
            var query = new StringBuilder("UPDATE Customer SET ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Name)) 
            {
                query.Append("name = @Name, ");
                parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            }

            if (!string.IsNullOrEmpty(request.Email)) 
            {
                query.Append("email = @Email, ");
                parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            }

            if (!string.IsNullOrEmpty(request.Phone))
            {
                query.Append("phone = @Phone, ");
                parameters.Add("Phone", request.Phone, DbType.AnsiStringFixedLength);
            }

            if (request.Birthday.HasValue || (request.IsBirthdayUpdated && request.Birthday is null))
            {
                query.Append("birthday = @Birthday, ");
                parameters.Add("Birthday", request.Birthday, DbType.Date);
            }

            if (query.ToString().EndsWith(", "))
            {
                query.Remove(query.Length - 2, 2);
            }

            query.Append(" WHERE id = @Id RETURNING *;");
            parameters.Add("Id", id, DbType.Int32);

            using (var connection = _connectionService.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Customer>(query.ToString(), parameters);
            }
        }
    }
}
