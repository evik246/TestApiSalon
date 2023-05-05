using TestApiSalon.Dtos;
using TestApiSalon.Models;

namespace TestApiSalon.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Result<string> GetPhotoURL(Employee employee);
        Task<Result<IEnumerable<Employee>>> GetAllEmployees();
        Task<Result<Employee>> GetEmployeeById(int id);
        Task<Result<Stream>> GetEmployeePhoto(int id);
    }
}
