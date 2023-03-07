using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using TestApiSalon.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext _context;
        private readonly IDbConnectionManager _connectionManager;
        private readonly IToken<Customer> _token;

        public CustomerService(DataContext context, IDbConnectionManager connectionManager, IToken<Customer> token)
        {
            _context = context;
            _connectionManager = connectionManager;
            _token = token;
        }

        public async Task<Customer> CreateCustomer(CustomerRegisterDto request)
        {
            var customer = await GetCustomerByEmail(request.Email);
            if (customer is not null)
            {
                throw new ArgumentException("Email is used");
            }

            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var query = "INSERT INTO Customer (name, birthday, email, password, phone) " +
                "values (@Name, @Birthday, @Email, @Password, @Phone)";

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                await connection.ExecuteAsync(query, request);
                
                var createdCustomer = await GetCustomerByEmail(request.Email);
                if (createdCustomer is null) 
                {
                    throw new OperationCanceledException("Customer cannot be created");
                }
                return createdCustomer;
            }
        }

        public async Task<string> LoginCustomer(UserLoginDto request)
        {
            var customer = await GetCustomerByEmail(request.Email);

            if (customer is null)
            {
                throw new ArgumentException("Invalid email");
            }

            if (BCrypt.Net.BCrypt.Verify(request.Password, customer.Password.Trim()) == false)
            {
                throw new ArgumentException("Invalid password");
            }

            return _token.Create(customer);
        }

        public async Task<Customer?> GetCustomerByEmail(string email)
        {
            var parameters = new
            {
                Email = email
            };

            var query = "SELECT * FROM Customer WHERE EMAIL = @Email";

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                var customers = await connection
                    .QueryAsync<Customer>(query, parameters);
                if (!customers.Any())
                {
                    return null;
                }
                return customers.First();
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
