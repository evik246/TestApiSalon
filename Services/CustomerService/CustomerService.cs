using Dapper;
using Npgsql;
using System.Data;
using System.Text;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
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

        public async Task<Result<Customer>> CreateCustomer(CustomerRegisterDto request)
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
                    var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                    return new Result<Customer>(customer);
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    return new Result<Customer>(new ConflictException("This email or phone number is already used"));
                }
            }
        }

        public async Task<Result<Customer>> GetCustomerByEmail(string email)
        {
            var parameters = new { Email = email };

            var query = "SELECT * FROM Customer WHERE email = @Email;";

            using (var connection = _connectionService.CreateConnection())
            {
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                if (customer is null)
                {
                    return new Result<Customer>(new NotFoundException("Client is not found"));
                }
                return new Result<Customer>(customer);
            }
        }

        public async Task<Result<Customer>> GetCustomerById(int id)
        {
            var parameters = new { Id = id };

            var query = "SELECT * FROM Customer WHERE id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                if (customer is null)
                {
                    return new Result<Customer>(new NotFoundException("Client is not found"));
                }
                return new Result<Customer>(customer);
            }
        }

        public async Task<Result<Customer>> UpdateCustomer(int id, CustomerUpdateDto request)
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

            if (request.IsBirthdayNullable || request.Birthday.HasValue)
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
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query.ToString(), parameters);
                if (customer is null)
                {
                    return new Result<Customer>(new NotFoundException("Client is not found"));
                }
                return new Result<Customer>(customer);
            }
        }
    }
}
