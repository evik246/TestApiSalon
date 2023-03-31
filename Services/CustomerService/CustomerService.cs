using Dapper;
using TestApiSalon.Dtos;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;
using TestApiSalon.Services.HashService;

namespace TestApiSalon.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IDbConnectionService _connectionService;
        private readonly IHashService _hashService;

        public CustomerService(IDbConnectionService connectionManager, IHashService hashService)
        {
            _connectionService = connectionManager;
            _hashService = hashService;
        }

        public async Task<Customer?> CreateCustomer(CustomerRegisterDto request)
        {
            var customer = await GetCustomerByEmail(request.Email);
            if (customer is not null)
            {
                return null;
            }

            request.Password = _hashService.Hash(request.Password);

            var query = "INSERT INTO Customer (name, birthday, email, password, phone) " +
                "values (@Name, @Birthday, @Email, @Password, @Phone)";

            using (var connection = _connectionService.CreateConnection())
            {
                await connection.ExecuteAsync(query, request);

                var createdCustomer = await GetCustomerByEmail(request.Email);
                return createdCustomer;
            }
        }

        public async Task<Customer?> GetCustomerByEmail(string email)
        {
            var parameters = new
            {
                Email = email
            };

            var query = "SELECT * FROM Customer WHERE EMAIL = @Email";

            using (var connection = _connectionService.CreateConnection())
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

            using (var connection = _connectionService.CreateConnection())
            {
                var customers = await connection
                    .QueryAsync<Customer>(query);
                return customers.ToList();
            }
        }
    }
}
