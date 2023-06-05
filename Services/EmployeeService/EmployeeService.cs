using Dapper;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Salon;
using TestApiSalon.Dtos.Service;
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

        public Result<string> GetPhotoURL(string? photopath, int employeeId)
        {
            if (!string.IsNullOrEmpty(photopath) && _contextAccessor.HttpContext is not null)
            {
                var url = _linkGenerator.GetUriByAction(_contextAccessor.HttpContext,
                    "GetEmployeePhoto", "Employee", new { id = employeeId });
                if (url is null)
                {
                    return new Result<string>(new NotFoundException("Photo path is not found"));
                }
                return new Result<string>(url);
            }
            return new Result<string>(new NotFoundException("Photo path is not found"));
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployees(Paging paging)
        {
            var parameters = new
            {
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT * FROM Employee OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query, parameters);

                foreach (var employee in employees)
                {
                    employee.PhotoURL = GetPhotoURL(employee.PhotoPath, employee.Id).Value;
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
                employee.PhotoURL = GetPhotoURL(employee.PhotoPath, employee.Id).Value;
                return new Result<Employee>(employee);
            }
        }

        public async Task<Result<Employee>> GetEmployeeByEmail(string email)
        {
            var parameters = new
            {
                Email = email
            };

            var query = "SELECT * FROM Employee WHERE email = @Email;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employee = await connection.QueryFirstOrDefaultAsync<Employee>(query, parameters);
                if (employee is null)
                {
                    return new Result<Employee>(new NotFoundException("Employee is not found"));
                }
                employee.PhotoURL = GetPhotoURL(employee.PhotoPath, employee.Id).Value;
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

            var query = "SELECT e.id, e.name, e.last_name, e.photo_path, e.specialization, "
                        + "s.id, s.name, s.price "
                        + "FROM Employee e "
                        + "JOIN Skill sk ON sk.employee_id = e.id "
                        + "JOIN Service s ON sk.service_id = s.id "
                        + "WHERE e.id = @Id;";

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
                    masterEntity.PhotoPath = GetPhotoURL(masterEntity.PhotoPath, masterEntity.Id).Value;
                    return masterEntity;
                }, param: parameters);

                var master = masters.FirstOrDefault();
                if (master is null)
                {
                    return new Result<MasterWithServicesDto>(new NotFoundException("Master is not found"));
                }
                return new Result<MasterWithServicesDto>(masters.First());
            }
        }

        public async Task<Result<IEnumerable<MasterWithServicesDto>>> GetAllMastersWithServices(int salonId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT e.id, e.name, e.last_name, e.photo_path, e.specialization, "
                        + "s.id, s.name, s.price "
                        + "FROM ( "
                        + "SELECT * FROM Employee e "
                        + "WHERE e.salon_id = @SalonId AND e.role = 'Master' "
                        + "OFFSET @Skip "
                        + "LIMIT @Take "
                        + ") e "
                        + "JOIN Skill sk ON sk.employee_id = e.id "
                        + "JOIN Service s ON sk.service_id = s.id "
                        + "ORDER BY e.id, s.price DESC;";

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
                        masterEntity.PhotoPath = GetPhotoURL(masterEntity.PhotoPath, masterEntity.Id).Value;
                        return masterEntity;
                    }, param: parameters);

                return new Result<IEnumerable<MasterWithServicesDto>>(masters.Distinct());
            }
        }

        public async Task<Result<MasterWithSalonDto>> GetMaster(int id)
        {
            var parameters = new
            {
                Id = id
            };

            var query = "SELECT e.id, e.name, e.last_name, e.email, "
                        + "e.specialization, e.photo_path, "
                        + "s.id, s.address "
                        + "FROM Employee e "
                        + "JOIN Salon s ON e.salon_id = s.id "
                        + "WHERE e.id = @Id;";

            using (var connection = _connectionService.CreateConnection
                ())
            {
                var masters = await connection.QueryAsync(
                    query, (MasterWithSalonDto master, SalonDto salon) =>
                    {
                        master.Salon = salon;
                        master.SalonId = salon.Id;
                        master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                        return master;
                    }, param: parameters
                );
                var master = masters.FirstOrDefault();
                if (master is null)
                {
                    return new Result<MasterWithSalonDto>(new NotFoundException("Master is not found"));
                }
                return new Result<MasterWithSalonDto>(master);
            }
        }

        public async Task<Result<MasterFullDto>> GetMasterById(int masterId)
        {
            var parameters = new
            {
                MasterId = masterId
            };

            var query = "SELECT e.id, e.name, e.last_name, "
                        + "e.specialization, e.photo_path "
                        + "FROM Employee e "
                        + "WHERE e.id = @MasterId "
                        + "AND e.role = 'Master';";

            using (var connection = _connectionService.CreateConnection())
            {
                var master = await connection.QueryFirstOrDefaultAsync<MasterFullDto>(query, parameters);
                if (master is null)
                {
                    return new Result<MasterFullDto>(new NotFoundException("Master is not found"));
                }
                return new Result<MasterFullDto>(master);
            }
        }

        public async Task<Result<IEnumerable<MasterDto>>> GetMastersWithNameByService(int salonId, int serviceId, Paging paging)
        {
            var parameters = new
            {
                ServiceId = serviceId,
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT e.id, e.name, e.last_name "
                        + "FROM Employee e "
                        + "JOIN Skill sk on sk.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND sk.service_id = @ServiceId "
                        + "ORDER BY e.last_name, e.name "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var masters = await connection.QueryAsync<MasterDto>(query, parameters);
                return new Result<IEnumerable<MasterDto>>(masters);
            }
        }
    }
}
