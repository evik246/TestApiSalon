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

            var query = "SELECT * FROM Employee WHERE e.id = @Id;";

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
    }
}
