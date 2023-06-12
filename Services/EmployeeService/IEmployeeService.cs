using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Result<string> GetPhotoURL(string? photopath, int employeeId);
        Task<Result<IEnumerable<Employee>>> GetAllEmployees(Paging paging, EmployeeFiltrationDto filtration);
        Task<Result<Employee>> GetEmployeeById(int id);
        Task<Result<Employee>> GetEmployeeByEmail(string email);
        Task<Result<Stream>> GetEmployeePhoto(int id);
        Task<Result<MasterWithServicesDto>> GetMasterWithServices(int id);
        Task<Result<IEnumerable<MasterWithServicesDto>>> GetAllMastersWithServices(int salonId, Paging paging);
        Task<Result<MasterWithSalonDto>> GetMaster(int id);
        Task<Result<MasterFullDto>> GetMasterById(int masterId);
        Task<Result<IEnumerable<MasterFullDto>>> GetMastersWithNameByService(int salonId, int serviceId, Paging paging);
        Task<Result<IEnumerable<MasterForManagerDto>>> GetManagerMasters(int salonId, Paging paging);
        Task<Result<IEnumerable<MasterForManagerDto>>> GetManagerMastersByService(int salonId, int serviceId, Paging paging);
        Task<Result<IEnumerable<MasterForManagerDto>>> GetManagerMastersByCategory(int salonId, int categoryId, Paging paging);
        Task<Result<MasterForManagerDto>> GetManagerMasterById(int salonId, int masterId);
        Task<Result<IEnumerable<MasterForManagerDto>>> GetAvailableMastersToChangeAnother(int salonId, int appointmentId);
        Task<Result<IEnumerable<MasterAppointmentCount>>> GetTopMasters(int salonId, int top);
        Task<Result<string>> CreateEmployee(EmployeeCreateDto request);
        Task<Result<string>> UpdateEmployee(int employeeId, EmployeeChangeDto request);
        Task<Result<string>> DeleteEmployee(int employeeId);
        Task<Result<string>> AddMasterService(int masterId, int serviceId);
        Task<Result<string>> RemoveMasterService(int masterId, int serviceId);
    }
}
