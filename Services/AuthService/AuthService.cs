using Dapper;
using Npgsql;
using System.Data;
using TestApiSalon.Data;
using TestApiSalon.Dtos.Auth;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services.ClaimsIdentityService;
using TestApiSalon.Services.ConnectionService;
using TestApiSalon.Services.CustomerService;
using TestApiSalon.Services.EmployeeService;
using TestApiSalon.Services.TokenService;

namespace TestApiSalon.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnectionService _connectionService;
        private readonly ITokenService _tokenService;
        
        private readonly IClaimsIdentityService<Customer> _customerIdentityService;
        private readonly IClaimsIdentityService<Employee> _employeeIdentityService;

        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;

        public AuthService(IDbConnectionService connectionService, ITokenService tokenService,
            ICustomerService customerService,
            IEmployeeService employeeService,
            IClaimsIdentityService<Customer> customerIdentityService, 
            IClaimsIdentityService<Employee> employeeIdentityService)
        {
            _connectionService = connectionService;
            _tokenService = tokenService;
            _customerService = customerService;
            _employeeService = employeeService;
            _customerIdentityService = customerIdentityService;
            _employeeIdentityService = employeeIdentityService;
        }

        public async Task<Result<string>> Login(UserLoginDto request)
        {
            _connectionService.ConnectionName = DbConnectionName.Default;

            var parameters = new
            {
                Email = request.Email,
                Password = request.Password
            };

            var query = "SELECT * FROM salon_users_schema.Users " +
                "WHERE email = @Email AND password = crypt(@Password, password);";

            User? user;
            using (var connection = _connectionService.CreateConnection())
            {
                user = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);

                if (user == null) 
                {
                    return new Result<string>(new UnauthorizedException("Invalid email or password"));
                }
            }

            _connectionService.ConnectionName = DbConnectionName.Guest;
            if (user.Role == UserRole.Client)
            {
                var customer = await _customerService.GetCustomerByEmail(user.Email);

                return customer.Match(c =>
                {
                    string token = _tokenService.CreateToken(_customerIdentityService.CreateClaimsIdentity(c));
                    return new Result<string>(token);
                }, 
                exception =>
                {
                    return new Result<string>(new NotFoundException("Customer is not found"));
                });
            }
            var employee = await _employeeService.GetEmployeeByEmail(user.Email);

            return employee.Match(e =>
            {
                string token = _tokenService.CreateToken(_employeeIdentityService.CreateClaimsIdentity(e));
                return new Result<string>(token);
            }, 
            exception =>
            {
                return new Result<string>(new NotFoundException("Employee is not found"));
            });
        }

        public async Task<Result<string>> ResetPassword(UserUpdatePasswordDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            parameters.Add("OldPassword", request.CurrentPassword, DbType.AnsiStringFixedLength);
            parameters.Add("NewPassword", request.NewPassword, DbType.AnsiStringFixedLength);

            var query = "CALL change_password(@Email, @OldPassword, @NewPassword);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Password is changed");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001")) 
                {
                    return new Result<string>(new UnauthorizedException(ex.MessageText));
                }
            }
        }
    }
}
