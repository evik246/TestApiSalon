using TestApiSalon.Models;

namespace TestApiSalon.Services.EmployeeService
{
    public interface IEmployeeService
    {
        string? GetPhotoURL(Employee employee);
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<Employee?> GetEmployeeById(int id);
        Task<Stream?> GetEmployeePhoto(int id);
    }
}
