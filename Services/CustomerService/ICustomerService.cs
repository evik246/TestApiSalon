using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer?> GetCustomerByEmail(string email);
        Task<Customer?> CreateCustomer(CustomerRegisterDto request);
        Task<string?> LoginCustomer(UserLoginDto request);
    }
}
