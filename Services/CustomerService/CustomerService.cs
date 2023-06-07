using Dapper;
using Npgsql;
using System.Data;
using System.Text;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IDbConnectionService _connectionService;

        public CustomerService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Result<string>> CreateCustomer(CustomerRegisterDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            parameters.Add("Password", request.Password, DbType.AnsiStringFixedLength);
            parameters.Add("Phone", request.Phone, DbType.AnsiStringFixedLength);
            parameters.Add("Birthday", request.Birthday, DbType.Date);

            var query = "CALL register_customer(@Name, @Email, @Password, @Phone, @Birthday);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                    return new Result<string>("Registration is successful");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("customer_email_key"))
                        {
                            return new Result<string>(new ConflictException("This email is already used"));
                        }
                        if (ex.ConstraintName.Equals("customer_phone_key"))
                        {
                            return new Result<string>(new ConflictException("This phone number is already used"));
                        }
                    }
                    return new Result<string>(new ConflictException("Invalid data"));
                }
            }
        }

        public async Task<Result<IEnumerable<Customer>>> GetAllCustomers(Paging paging)
        {
            var parameters = new
            {
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT * FROM Customer ORDER BY name OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var customers = await connection.QueryAsync<Customer>(query, parameters);
                return new Result<IEnumerable<Customer>>(customers);
            }
        }

        public async Task<Result<Customer>> GetCustomerByEmail(string email)
        {
            var parameters = new { Email = email };

            var query = "SELECT * FROM Customer WHERE email = @Email;";

            using (var connection = _connectionService.CreateConnection())
            {
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                if (customer is null)
                {
                    return new Result<Customer>(new NotFoundException("Client is not found"));
                }
                return new Result<Customer>(customer);
            }
        }

        public async Task<Result<Customer>> GetCustomerById(int id)
        {
            var parameters = new { Id = id };

            var query = "SELECT * FROM Customer WHERE id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query, parameters);
                if (customer is null)
                {
                    return new Result<Customer>(new NotFoundException("Client is not found"));
                }
                return new Result<Customer>(customer);
            }
        }

        public async Task<Result<CustomerAppointmentDate>> GetFirstCustomerAppointmentDate(int salonId, int customerId)
        {
            var parameters = new
            {
                SalonId = salonId,
                CustomerId = customerId
            };

            var query = "SELECT MIN(a.date)::DATE AS appointment_date, "
                        + "DATE_PART('day', CURRENT_DATE - MIN(a.date)) AS duration "
                        + "FROM Appointment a "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "WHERE a.customer_id = @CustomerId AND e.salon_id = @SalonId "
                        + "AND a.status = 'Completed';";

            using (var connection = _connectionService.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, parameters);

                var firstDate = new CustomerAppointmentDate
                {
                    AppointmentDate = result.appointment_date != null ? 
                        DateOnly.FromDateTime(result.appointment_date) : null,
                    Duration = (int?) result.duration
                };
                return new Result<CustomerAppointmentDate>(firstDate);
            }
        }

        public async Task<Result<CustomerAppointmentDate>> GetLastCustomerAppointmentDate(int salonId, int customerId)
        {
            var parameters = new
            {
                SalonId = salonId,
                CustomerId = customerId
            };

            var query = "SELECT MAX(a.date)::DATE AS appointment_date, "
                        + "DATE_PART('day', CURRENT_DATE - MAX(a.date)) AS duration "
                        + "FROM Appointment a "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "WHERE a.customer_id = @CustomerId AND e.salon_id = @SalonId "
                        + "AND a.status = 'Completed';";

            using (var connection = _connectionService.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync(query, parameters);
                var lastDate = new CustomerAppointmentDate
                {
                    AppointmentDate = result.appointment_date != null ?
                        DateOnly.FromDateTime(result.appointment_date) : null,
                    Duration = (int?)result.duration
                };
                return new Result<CustomerAppointmentDate>(lastDate);
            }
        }

        public async Task<Result<string>> ResetPassword(string email, CustomerChangePassword request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", email, DbType.AnsiStringFixedLength);
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

        public async Task<Result<Customer>> UpdateCustomer(int id, CustomerUpdateDto request)
        {
            var query = new StringBuilder("UPDATE Customer SET ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Name)) 
            {
                query.Append("name = @Name, ");
                parameters.Add("Name", request.Name, DbType.AnsiStringFixedLength);
            }

            if (!string.IsNullOrEmpty(request.Email)) 
            {
                query.Append("email = @Email, ");
                parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            }

            if (!string.IsNullOrEmpty(request.Phone))
            {
                query.Append("phone = @Phone, ");
                parameters.Add("Phone", request.Phone, DbType.AnsiStringFixedLength);
            }

            if (request.IsBirthdayNullable || request.Birthday.HasValue)
            {
                query.Append("birthday = @Birthday, ");
                parameters.Add("Birthday", request.Birthday, DbType.Date);
            }

            if (query.ToString().EndsWith(", "))
            {
                query.Remove(query.Length - 2, 2);
            }

            query.Append(" WHERE id = @Id RETURNING *;");
            parameters.Add("Id", id, DbType.Int32);

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query.ToString(), parameters);
                    if (customer is null)
                    {
                        return new Result<Customer>(new NotFoundException("Client is not found"));
                    }
                    return new Result<Customer>(customer);
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("customer_email_key"))
                        {
                            return new Result<Customer>(new ConflictException("This email is already used"));
                        }
                        if (ex.ConstraintName.Equals("customer_phone_key"))
                        {
                            return new Result<Customer>(new ConflictException("This phone number is already used"));
                        }
                    }
                    return new Result<Customer>(new ConflictException("Invalid data"));
                }
            }
        }
    }
}
