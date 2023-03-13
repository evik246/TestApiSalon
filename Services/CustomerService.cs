using Dapper;
using TestApiSalon.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DataContext _context;
        private readonly IDbConnectionManager _connectionManager;
        private readonly ITokenGeneratorService<Customer> _tokenGenerator;
        private readonly IHashService _hashService;

        public CustomerService(DataContext context, IDbConnectionManager connectionManager, ITokenGeneratorService<Customer> token, IHashService hashService)
        {
            _context = context;
            _connectionManager = connectionManager;
            _tokenGenerator = token;
            _hashService = hashService;
        }

        public async Task<Customer> CreateCustomer(CustomerRegisterDto request)
        {
            var customer = await GetCustomerByEmail(request.Email);
            if (customer is not null)
            {
                throw new ConflictException("Email is used");
            }

            request.Password = _hashService.Hash(request.Password);

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
                throw new UnauthorizedException("Invalid email");
            }

            if (_hashService.Verify(customer.Password.Trim(), request.Password) == false)
            {
                throw new UnauthorizedException("Invalid password");
            }

            return _tokenGenerator.CreateToken(customer);
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
