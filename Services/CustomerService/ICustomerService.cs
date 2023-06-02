using TestApiSalon.Dtos.Auth;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<Result<string>> CreateCustomer(CustomerRegisterDto request);
        Task<Result<Customer>> GetCustomerById(int id);
        Task<Result<Customer>> GetCustomerByEmail(string email);
        Task<Result<Customer>> UpdateCustomer(int id, CustomerUpdateDto request);
        Task<Result<string>> ResetPassword(string email, CustomerChangePassword request);
    }
}
