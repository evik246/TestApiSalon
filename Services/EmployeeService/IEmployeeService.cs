using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Result<string> GetPhotoURL(string? photopath, int employeeId);
        Task<Result<IEnumerable<Employee>>> GetAllEmployees(Paging paging);
        Task<Result<Employee>> GetEmployeeById(int id);
        Task<Result<Stream>> GetEmployeePhoto(int id);
        Task<Result<MasterWithServicesDto>> GetMasterWithServices(int id);
        Task<Result<IEnumerable<MasterWithServicesDto>>> GetAllMastersWithServices(int salonId, Paging paging);
    }
}
