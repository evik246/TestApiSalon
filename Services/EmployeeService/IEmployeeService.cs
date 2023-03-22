using TestApiSalon.Models;

namespace TestApiSalon.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployees();
    }
}
