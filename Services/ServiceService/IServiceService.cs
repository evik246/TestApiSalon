using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Service;
using TestApiSalon.Models;

namespace TestApiSalon.Services.ServiceService
{
    public interface IServiceService
    {
        Task<Result<IEnumerable<Service>>> GetAllServices(Paging paging);
        Task<Result<Service>> GetServiceById(int id);
        Task<Result<ServiceWithoutCategoryDto>> GetServiceWithoutCategoryById(int serviceId);
        Task<Result<IEnumerable<Service>>> GetMasterServices(int masterId, Paging paging);
        Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetMasterServicesByCategory(int masterId, int categoryId, Paging paging);
        Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetMasterServicesByCategoryAndSalon(int salonId, int masterId, int categoryId, Paging paging);
        Task<Result<IEnumerable<Service>>> GetServicesInSalon(int salonId, Paging paging);
        Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetServicesInSalonByCategory(int salonId, int categoryId, Paging paging);
        Task<Result<IEnumerable<ServiceWithoutCategoryDto>>> GetAllServicesByCategory(int categoryId, Paging paging);
        Task<Result<IEnumerable<ServiceAppointmentCount>>> GetTopServices(int salonId, int top);
    }
}