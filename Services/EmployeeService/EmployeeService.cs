using Dapper;
using Npgsql;
using System.Data;
using System.Text;
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

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployees(Paging paging, EmployeeFiltrationDto filtration)
        {
            var query = new StringBuilder("SELECT e.id, e.name, e.last_name, e.role, e.photo_path, e.email, e.specialization FROM Employee e WHERE ");

            var parameters = new DynamicParameters();
            parameters.Add("Skip", paging.Skip);
            parameters.Add("Take", paging.PageSize);

            if (filtration.SalonId != null)
            {
                query.Append("e.salon_id = @SalonId AND ");
                parameters.Add("SalonId", filtration.SalonId);
            }

            if (filtration.Role != null)
            {
                query.Append("e.role = @Role AND ");
                parameters.Add("Role", filtration.Role?.ToString());
            }

            if (query.ToString().EndsWith("AND "))
            {
                query.Remove(query.Length - 4, 4);
            }

            if (query.ToString().EndsWith("WHERE "))
            {
                query.Remove(query.Length - 6, 6);
            }

            query.Append(" ORDER BY e.last_name, e.name OFFSET @Skip LIMIT @Take;");

            using (var connection = _connectionService.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query.ToString(), parameters);

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

            var query = "SELECT e.id, e.name, e.last_name, e.role, e.photo_path, e.email, e.specialization FROM Employee e WHERE id = @Id;";

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
                master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                return new Result<MasterFullDto>(master);
            }
        }

        public async Task<Result<IEnumerable<MasterFullDto>>> GetMastersWithNameByService(int salonId, int serviceId, Paging paging)
        {
            var parameters = new
            {
                ServiceId = serviceId,
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT e.id, e.name, e.last_name, "
                        + "e.specialization, e.photo_path "
                        + "FROM Employee e "
                        + "JOIN Skill sk on sk.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND sk.service_id = @ServiceId "
                        + "ORDER BY e.last_name, e.name "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var masters = await connection.QueryAsync<MasterFullDto>(query, parameters);
                foreach (var master in masters)
                {
                    master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                }
                return new Result<IEnumerable<MasterFullDto>>(masters);
            }
        }

        public async Task<Result<IEnumerable<MasterForManagerDto>>> GetManagerMasters(int salonId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT e.id, e.name, e.last_name, e.email, "
                        + "e.specialization, e.photo_path "
                        + "FROM Employee e "
                        + "WHERE e.salon_id = @SalonId AND e.role = 'Master' "
                        + "ORDER BY e.last_name, e.name "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var masters = await connection.QueryAsync<MasterForManagerDto>(query, parameters);
                foreach (var master in masters)
                {
                    master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                }
                return new Result<IEnumerable<MasterForManagerDto>>(masters);
            }
        }

        public async Task<Result<IEnumerable<MasterForManagerDto>>> GetManagerMastersByService(int salonId, int serviceId, Paging paging)
        {
            var parameters = new
            {
                ServiceId = serviceId,
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT e.id, e.name, e.last_name, e.email, "
                        + "e.specialization, e.photo_path "
                        + "FROM Employee e "
                        + "JOIN Skill sk on sk.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND sk.service_id = @ServiceId "
                        + "ORDER BY e.last_name, e.name "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var masters = await connection.QueryAsync<MasterForManagerDto>(query, parameters);
                foreach (var master in masters)
                {
                    master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                }
                return new Result<IEnumerable<MasterForManagerDto>>(masters);
            }
        }

        public async Task<Result<IEnumerable<MasterForManagerDto>>> GetManagerMastersByCategory(int salonId, int categoryId, Paging paging)
        {
            var parameters = new
            {
                CategoryId = categoryId,
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT DISTINCT e.id, e.name, e.last_name, e.email, "
                        + "e.specialization, e.photo_path "
                        + "FROM Employee e "
                        + "JOIN Skill sk on sk.employee_id = e.id "
                        + "JOIN Service s ON sk.service_id = s.id "
                        + "WHERE e.salon_id = @SalonId AND s.category_id = @CategoryId "
                        + "ORDER BY e.last_name, e.name "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var masters = await connection.QueryAsync<MasterForManagerDto>(query, parameters);
                foreach (var master in masters)
                {
                    master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                }
                return new Result<IEnumerable<MasterForManagerDto>>(masters);
            }
        }

        public async Task<Result<MasterForManagerDto>> GetManagerMasterById(int salonId, int masterId)
        {
            var parameters = new
            {
                MasterId = masterId,
                SalonId = salonId,
            };

            var query = "SELECT e.id, e.name, e.last_name, e.email, "
                        + "e.specialization, e.photo_path "
                        + "FROM Employee e "
                        + "WHERE e.salon_id = @SalonId AND e.role = 'Master' AND e.id = @MasterId;";

            using (var connection = _connectionService.CreateConnection()) 
            {
                var master = await connection.QueryFirstOrDefaultAsync<MasterForManagerDto>(query, parameters);
                if (master == null)
                {
                    return new Result<MasterForManagerDto>(new NotFoundException("Master is not found"));
                }
                master.PhotoPath = GetPhotoURL(master.PhotoPath, master.Id).Value;
                return new Result<MasterForManagerDto>(master);
            }
        }

        public async Task<Result<IEnumerable<MasterForManagerDto>>> GetAvailableMastersToChangeAnother(int salonId, int appointmentId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("SalonId", salonId, DbType.Int32);
            parameters.Add("AppointmentId", appointmentId, DbType.Int32);

            var query = "SELECT * FROM get_available_masters(@SalonId, @AppointmentId);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    var masters = await connection.QueryAsync<MasterForManagerDto>(query, parameters);
                    return new Result<IEnumerable<MasterForManagerDto>>(masters);
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001"))
                {
                    if (ex.MessageText.Contains("is not found"))
                    {
                        return new Result<IEnumerable<MasterForManagerDto>>(new NotFoundException(ex.MessageText));
                    }
                    return new Result<IEnumerable<MasterForManagerDto>>(new ConflictException(ex.MessageText));
                }
            }
        }

        public async Task<Result<IEnumerable<MasterAppointmentCount>>> GetTopMasters(int salonId, int top)
        {
            var parameters = new
            {
                SalonId = salonId,
                Top = top
            };

            var query = "SELECT e.id, e.name, e.last_name, "
                        + "e.specialization, e.photo_path, "
                        + "COUNT(*) AS appointments_count "
                        + "FROM Appointment a "
                        + "JOIN Employee e ON e.id = a.employee_id "
                        + "WHERE e.salon_id = @SalonId "
                        //+ "AND a.status = 'Completed' "
                        + "GROUP BY e.id, e.name, e.last_name, "
                        + "e.specialization, e.photo_path "
                        + "ORDER BY appointments_count DESC "
                        + "LIMIT @Top;";

            using (var connection = _connectionService.CreateConnection()) 
            {
                var masters = await connection.QueryAsync<MasterAppointmentCount>(query, parameters);
                return new Result<IEnumerable<MasterAppointmentCount>>(masters);
            }
        }

        public async Task<Result<string>> CreateEmployee(EmployeeCreateDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            parameters.Add("LastName", request.LastName, DbType.AnsiStringFixedLength);
            parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            parameters.Add("Password", request.Password, DbType.AnsiStringFixedLength);
            parameters.Add("Role", request.Role.ToString(), DbType.AnsiStringFixedLength);
            parameters.Add("SalonId", request.SalonId, DbType.Int32);
            parameters.Add("Specialization", request.Specialization, DbType.AnsiStringFixedLength);

            var query = "CALL register_employee(@Name, @LastName, @Email, @Password, @Role, @SalonId, NULL, @Specialization);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Employee is created");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("employee_email_key"))
                        {
                            return new Result<string>(new ConflictException("This email is already used"));
                        }
                    }
                    return new Result<string>(new ConflictException("Invalid data"));
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001"))
                {
                    return new Result<string>(new ConflictException(ex.MessageText));
                }
            }
        }

        public async Task<Result<string>> UpdateEmployee(int employeeId, EmployeeChangeDto request)
        {
            var query = new StringBuilder("UPDATE Employee SET ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query.Append("name = @Name, ");
                parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                query.Append("last_name = @LastName, ");
                parameters.Add("LastName", request.LastName, DbType.AnsiStringFixedLength);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                query.Append("email = @Email, ");
                parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            }

            if (request.Role != null)
            {
                query.Append("role = @Role, ");
                parameters.Add("Role", request.Role.ToString(), DbType.AnsiStringFixedLength);
            }

            if (request.IsSalonIdNullable || request.SalonId > 0) 
            {
                query.Append("salon_id = @SalonId, ");
                parameters.Add("SalonId", request.SalonId, DbType.Int32);
            }

            if (request.IsSpecializationNullable || !string.IsNullOrEmpty(request.Specialization))
            {
                query.Append("specialization = @Specialization, ");
                parameters.Add("Specialization", request.Specialization, DbType.AnsiStringFixedLength);
            }

            if (query.ToString().EndsWith(", "))
            {
                query.Remove(query.Length - 2, 2);
            }

            query.Append(" WHERE id = @Id;");
            parameters.Add("Id", employeeId, DbType.Int32);

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    int rows = await connection.ExecuteAsync(query.ToString(), parameters);
                    if (rows == 0)
                    {
                        return new Result<string>(new NotFoundException("Employee is not found"));
                    }
                    return new Result<string>("Employee is changed");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("employee_email_key"))
                        {
                            return new Result<string>(new ConflictException("This email is already used"));
                        }
                    }
                    return new Result<string>(new ConflictException("Invalid data"));
                }
            }
        }

        public async Task<Result<string>> DeleteEmployee(int employeeId)
        {
            var parameters = new
            {
                Id = employeeId
            };

            var query = "DELETE FROM Employee WHERE id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    int rows = await connection.ExecuteAsync(query, parameters);
                    if (rows == 0)
                    {
                        return new Result<string>(new NotFoundException("Employee is not found"));
                    }
                    return new Result<string>("Employee is deleted");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23503"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("schedule_employee_id_fkey"))
                        {
                            return new Result<string>(new ConflictException("This employee has schedule"));
                        }
                    }
                    return new Result<string>(new ConflictException("Unable to delete the employee"));
                }
            }
        }

        public async Task<Result<string>> AddMasterService(int masterId, int serviceId)
        {
            var parameters = new
            {
                MasterId = masterId,
                ServiceId = serviceId
            };

            var query = "INSERT INTO Skill (employee_id, service_id) VALUES "
                        + "(@MasterId, @ServiceId);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Service is added to master");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23503"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("skill_service_id_fkey"))
                        {
                            return new Result<string>(new NotFoundException("Service is not found"));
                        }
                    }
                    return new Result<string>(new ConflictException("Unable to add service to the master"));
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23503"))
                {
                    return new Result<string>(new ConflictException(ex.Message));
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("skill_pkey"))
                        {
                            return new Result<string>(new ConflictException("Service is already added to master"));
                        }
                    }
                    return new Result<string>(new ConflictException("Unable to add service to the master"));
                }
            }
        }

        public async Task<Result<string>> RemoveMasterService(int masterId, int serviceId)
        {
            var parameters = new
            {
                MasterId = masterId,
                ServiceId = serviceId
            };

            var query = "DELETE FROM Skill WHERE employee_id = @MasterId AND service_id = @ServiceId;";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    int rows = await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Service is removed to master");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23503"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("skill_service_id_fkey"))
                        {
                            return new Result<string>(new NotFoundException("Service is not found"));
                        }
                    }
                    return new Result<string>(new ConflictException("Unable to remove service to the master"));
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23503"))
                {
                    return new Result<string>(new ConflictException(ex.Message));
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("skill_pkey"))
                        {
                            return new Result<string>(new ConflictException("Service is already removed to master"));
                        }
                    }
                    return new Result<string>(new ConflictException("Unable to remove service to the master"));
                }
            }
        }
    }
}
