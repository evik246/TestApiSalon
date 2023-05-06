using Dapper;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;
using TestApiSalon.Services.FileService;

namespace TestApiSalon.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDbConnectionService _connectionService;
        private readonly IFileService _fileService;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _contextAccessor;

        public EmployeeService(IDbConnectionService connectionService,
            IFileService fileService,
            LinkGenerator linkGenerator,
            IHttpContextAccessor contextAccessor)
        {
            _connectionService = connectionService;
            _linkGenerator = linkGenerator;
            _fileService = fileService;
            _contextAccessor = contextAccessor;
        }

        public Result<string> GetPhotoURL(Employee employee)
        {
            if (!string.IsNullOrEmpty(employee.PhotoPath) && _contextAccessor.HttpContext is not null)
            {
                var url = _linkGenerator.GetUriByAction(_contextAccessor.HttpContext,
                    "GetEmployeePhoto", "Employee", new { id = employee.Id });
                if (url is null)
                {
                    return new Result<string>(new NotFoundException("Photo path is not found"));
                }
                return new Result<string>(url);
            }
            return new Result<string>(new NotFoundException("Photo path is not found"));
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployees()
        {
            var query = "SELECT * FROM Employee;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);

                foreach (var employee in employees)
                {
                    employee.PhotoURL = GetPhotoURL(employee).Value;
                }
                return new Result<IEnumerable<Employee>>(employees);
            }
        }

        public async Task<Result<Employee>> GetEmployeeById(int id)
        {
            var parameters = new
            {
                Id = id
            };

            var query = "SELECT * FROM Employee WHERE id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employee = await connection.QueryFirstOrDefaultAsync<Employee>(query, parameters);
                if (employee is null) 
                {
                    return new Result<Employee>(new NotFoundException("Employee is not found"));
                }
                employee.PhotoURL = GetPhotoURL(employee).Value;
                return new Result<Employee>(employee);
            }
        }

        public async Task<Result<Stream>> GetEmployeePhoto(int id)
        {
            Result<Employee> employee = await GetEmployeeById(id);

            if (employee.Value is null || string.IsNullOrEmpty(employee.Value.PhotoPath))
            {
                return new Result<Stream>(new NotFoundException("Photo of the employee is not found"));
            }

            var stream = await _fileService.DownloadFile(employee.Value.PhotoPath);

            return stream.Match(result =>
            {
                return new Result<Stream>(result);
            }, exception =>
            {
                return new Result<Stream>(new NotFoundException("Photo of the employee is not found"));
            });
        }

        public async Task<Result<MasterWithServicesDto>> GetMasterWithServices(int id)
        {
            var parameters = new
            {
                Id = id
            };

            var query = "SELECT e.id, e.name, e.last_name, e.photo_path, e.specialization, " +
                "s.id, s.name, s.price " +
                "FROM Employee e " +
                "JOIN Skill sk ON sk.employee_id = e.id " +
                "JOIN Service s ON sk.service_id = s.id " +
                "WHERE e.id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var temp = new Dictionary<int, MasterWithServicesDto>();
                var masters = await connection.QueryAsync
                    (query, (MasterWithServicesDto master, ServiceDto service) =>
                {
                    MasterWithServicesDto masterEntity;
                    if (!temp.TryGetValue(master.Id, out masterEntity!))
                    {
                        temp.Add(master.Id, masterEntity = master);
                    }

                    if (masterEntity.Services == null) 
                    {
                        masterEntity.Services = new List<ServiceDto>();
                    }

                    if (service != null)
                    {
                        if (!masterEntity.Services.Any(s => s.Id == service.Id))
                        {
                            masterEntity.Services.Add(service);
                        }
                    }
                    return masterEntity;
                }, param: parameters);

                if (!masters.Any())
                {
                    return new Result<MasterWithServicesDto>(new NotFoundException("Master is not found"));
                }
                return new Result<MasterWithServicesDto>(masters.First());
            }
        }

        public async Task<Result<IEnumerable<MasterWithServicesDto>>> GetAllMastersWithServices(int salonId)
        {
            var parameters = new
            {
                SalonId = salonId
            };

            var query = "SELECT e.id, e.name, e.last_name, e.photo_path, e.specialization, " +
                "s.id, s.name, s.price " +
                "FROM Employee e " +
                "JOIN Skill sk ON sk.employee_id = e.id " +
                "JOIN Service s ON sk.service_id = s.id " +
                "WHERE e.salon_id = @SalonId AND e.role = 'Master' " +
                "ORDER BY s.price DESC;";

            using (var connection = _connectionService.CreateConnection())
            {
                var temp = new Dictionary<int, MasterWithServicesDto>();
                var masters = await connection.QueryAsync
                    (query, (MasterWithServicesDto master, ServiceDto service) =>
                    {
                        MasterWithServicesDto masterEntity;
                        if (!temp.TryGetValue(master.Id, out masterEntity!))
                        {
                            temp.Add(master.Id, masterEntity = master);
                        }

                        if (masterEntity.Services == null)
                        {
                            masterEntity.Services = new List<ServiceDto>();
                        }

                        if (service != null)
                        {
                            if (!masterEntity.Services.Any(s => s.Id == service.Id))
                            {
                                masterEntity.Services.Add(service);
                            }
                        }
                        return masterEntity;
                    }, param: parameters);
                return new Result<IEnumerable<MasterWithServicesDto>>(masters.Distinct());
            }
        }
    }
}
