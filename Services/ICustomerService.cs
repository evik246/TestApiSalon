using TestApiSalon.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public interface ICustomerService : IDbConnectionBase
    {
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer> GetCustomerByEmail(string email);
        Task<Customer> CreateCustomer(CustomerRequestDto request);
    }
}
