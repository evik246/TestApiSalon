using Dapper;
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

        public string? GetPhotoURL(Employee employee)
        {
            if (!string.IsNullOrEmpty(employee.PhotoPath) && _contextAccessor.HttpContext is not null)
            {
                return _linkGenerator.GetUriByAction(_contextAccessor.HttpContext,
                    "GetEmployeePhoto", "Employee", new { id = employee.Id });
            }
            return null;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            var query = "SELECT * FROM Employee;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);

                foreach (var employee in employees)
                {
                    employee.PhotoURL = GetPhotoURL(employee);
                }
                return employees.ToList();
            }
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            var parameters = new
            {
                Id = id
            };

            var query = "SELECT * FROM Employee WHERE e.id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query, parameters);
                if (!employees.Any()) 
                {
                    return null;
                }

                var employee = employees.First();
                employee.PhotoURL = GetPhotoURL(employee);

                return employee;
            }
        }

        public async Task<Stream?> GetEmployeePhoto(int id)
        {
            var employee = await GetEmployeeById(id);
            if (employee is null || string.IsNullOrEmpty(employee.PhotoPath))
            {
                return null;
            }
            return await _fileService.DownloadFile(employee.PhotoPath);
        }
    }
}
