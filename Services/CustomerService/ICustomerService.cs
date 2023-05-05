using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<Result<Customer>> CreateCustomer(CustomerRegisterDto request);
        Task<Result<Customer>> GetCustomerById(int id);
        Task<Result<Customer>> GetCustomerByEmail(string email);
        Task<Result<Customer>> UpdateCustomer(int id, CustomerUpdateDto request);
    }
}
