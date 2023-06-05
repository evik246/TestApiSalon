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
        Task<Result<Employee>> GetEmployeeByEmail(string email);
        Task<Result<Stream>> GetEmployeePhoto(int id);
        Task<Result<MasterWithServicesDto>> GetMasterWithServices(int id);
        Task<Result<IEnumerable<MasterWithServicesDto>>> GetAllMastersWithServices(int salonId, Paging paging);
        Task<Result<MasterWithSalonDto>> GetMaster(int id);
        Task<Result<MasterFullDto>> GetMasterById(int masterId);
        Task<Result<IEnumerable<MasterDto>>> GetMastersWithNameByService(int salonId, int serviceId, Paging paging);
    }
}
