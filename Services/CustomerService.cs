using BCrypt.Net;
using Dapper;
using TestApiSalon.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext _context;
        private readonly IDbConnectionManager _connectionManager;

        public CustomerService(DataContext context, IDbConnectionManager connectionManager)
        {
            _context = context;
            _connectionManager = connectionManager;
        }

        public async Task<Customer> CreateCustomer(CustomerRequestDto request)
        {
            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var query = "INSERT INTO Customer (name, birthday, email, password, phone) " +
                "values (@Name, @Birthday, @Email, @Password, @Phone)";

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                await connection.ExecuteAsync(query, request);
                return await GetCustomerByEmail(request.Email);
            }
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            var parameters = new
            {
                Email = email
            };

            var query = "SELECT * FROM Customer WHERE EMAIL = @Email LIMIT 1";

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                var customer = await connection
                    .QueryAsync<Customer>(query, parameters);
                return customer.First();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            var query = "SELECT * FROM Customer";

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                var customers = await connection
                    .QueryAsync<Customer>(query);
                return customers.ToList();
            }
        }
    }
}
