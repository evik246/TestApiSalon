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
        private readonly ITokenService _tokenService;
        private readonly IClaimsIdentityService<Customer> _identityService;
        private readonly IHashService _hashService;

        public CustomerService(DataContext context, IDbConnectionManager connectionManager, 
            ITokenService tokenService, 
            IHashService hashService, 
            IClaimsIdentityService<Customer> identityService)
        {
            _context = context;
            _connectionManager = connectionManager;
            _tokenService = tokenService;
            _hashService = hashService;
            _identityService = identityService;
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

            using (var connection = _context.CreateConnection(_connectionManager.ConnectionName))
            {
                await connection.ExecuteAsync(query, request);
                
                var createdCustomer = await GetCustomerByEmail(request.Email);
                return createdCustomer;
            }
        }

        public async Task<string?> LoginCustomer(UserLoginDto request)
        {
            var customer = await GetCustomerByEmail(request.Email);

            if (customer is null)
            {
                return null;
            }

            if (_hashService.Verify(customer.Password.Trim(), request.Password) == false)
            {
                return null;
            }

            return _tokenService.CreateToken(_identityService.CreateClaimsIdentity(customer));
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
