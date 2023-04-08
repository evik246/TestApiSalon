using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CustomerService
{
    public interface ICustomerService
    {
        /*Task<IEnumerable<Customer>> GetAllCustomers();
        Task<Customer?> GetCustomerByEmail(string email);
        Task<Customer?> CreateCustomer(CustomerRegisterDto request);*/

        Task<Customer?> CreateCustomer(CustomerRegisterDto request);
        Task<Customer?> GetCustomerById(int id);
        Task<Customer?> GetCustomerByEmail(string email);
        Task<Customer?> UpdateCustomer(int id, CustomerUpdateDto request);
    }
}
