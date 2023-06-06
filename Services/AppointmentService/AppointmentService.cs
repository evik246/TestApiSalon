using Dapper;
using Npgsql;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;
using TestApiSalon.Dtos.Service;
using TestApiSalon.Exceptions;
using TestApiSalon.Extensions;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IDbConnectionService _connectionService;

        public AppointmentService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments
            (int customerId, int salonId, Paging paging)
        {
            var parameters = new
            {
                CustomerId = customerId,
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT a.id, a.date, "
                        + "s.id, s.name, s.price, "
                        + "e.id, e.name, e.last_name "
                        + "FROM Appointment a "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "JOIN Salon sa ON e.salon_id = sa.id "
                        + "WHERE a.customer_id = @CustomerId "
                        + "AND sa.id = @SalonId "
                        + "AND a.status = 'Active' "
                        + "ORDER BY a.date "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    query, (CustomerAppointmentDto appointment,
                            ServiceDto service,
                            MasterDto master) =>
                    {
                        appointment.Master = master;
                        appointment.Service = service;
                        return appointment;
                    }, param: parameters
                );
                return new Result<IEnumerable<CustomerAppointmentDto>>(appointments);
            }
        }

        public async Task<Result<CustomerAppointmentDto>> GetCustomerAppointmentById(int customerId, int appointmentId)
        {
            var parameters = new
            {
                CustomerId = customerId,
                AppointmentId = appointmentId
            };

            var query = "SELECT a.id, a.date, "
                        + "s.id, s.name, s.price, "
                        + "e.id, e.name, e.last_name "
                        + "FROM Appointment a "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "JOIN Salon sa ON e.salon_id = sa.id "
                        + "WHERE a.customer_id = @CustomerId "
                        + "AND a.status = 'Active' "
                        + "AND a.id = @AppointmentId;";

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    query, (CustomerAppointmentDto appointment,
                            ServiceDto service,
                            MasterDto master) =>
                    {
                        appointment.Master = master;
                        appointment.Service = service;
                        return appointment;
                    }, param: parameters
                );
                if (!appointments.Any())
                {
                    return new Result<CustomerAppointmentDto>(new NotFoundException("Appointment is not found"));
                }
                return new Result<CustomerAppointmentDto>(appointments.First());
            }
        }

        public async Task<Result<string>> CreateAppointment(int customerId, AppointmentCreateDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Date", request.Date.ToCorrectDateTime(), DbType.DateTime2);
            parameters.Add("CustomerId", customerId, DbType.Int32);
            parameters.Add("ServiceId", request.ServiceId, DbType.Int32);
            parameters.Add("EmployeeId", request.EmployeeId, DbType.Int32);

            var query = "INSERT INTO appointment (date, customer_id, service_id, employee_id) " +
                "VALUES (@Date, @CustomerId, @ServiceId, @EmployeeId);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.QueryAsync(query, parameters);
                    return new Result<string>("Appointment is created successfully");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001"))
                {
                    return new Result<string>(new ConflictException(ex.MessageText));
                }
            }
        }

        public async Task<Result<string>> CancelAppointment(int customerId, int appointmentId)
        {
            var parameters = new
            {
                CustomerId = customerId,
                AppointmentId = appointmentId
            };

            var query = "DELETE FROM Appointment "
                        + "WHERE id = @AppointmentId AND customer_id = @CustomerId;";

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("Customer or appointment is not found"));
                }
                return new Result<string>("Appointment deleted successfully");
            }
        }

        public async Task<Result<IEnumerable<MasterAppointmentDto>>> GetMasterAppintments
            (int masterId, Paging paging, int? customerId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("MasterId", masterId, DbType.Int32);
            parameters.Add("Skip", paging.Skip, DbType.Int32);
            parameters.Add("Take", paging.PageSize, DbType.Int32);

            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT a.id, a.date, ");
            builder.Append("s.id, s.name, ");
            builder.Append("c.id, c.name ");
            builder.Append("FROM Appointment a ");
            builder.Append("JOIN Service s ON a.service_id = s.id ");
            builder.Append("JOIN Customer c ON a.customer_id = c.id ");
            builder.Append("WHERE a.employee_id = @MasterId ");
            builder.Append("AND a.status = 'Active' ");

            if (customerId != null)
            {
                builder.Append("AND a.customer_id = @CustomerId ");
                parameters.Add("CustomerId", customerId, DbType.Int32);
            }

            builder.Append("ORDER BY a.date ");
            builder.Append("OFFSET @Skip LIMIT @Take;");

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    builder.ToString(), 
                    (MasterAppointmentDto appointment,
                    ServiceNameDto service,
                    CustomerDto customer) =>
                    {
                        appointment.Service = service;
                        appointment.Customer = customer;
                        return appointment;
                    }, param: parameters
                );
                return new Result<IEnumerable<MasterAppointmentDto>>(appointments);
            }
        }

        public async Task<Result<string>> MarkMasterAppointmentCompleted(int masterId, int appointmentId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("MasterId", masterId, DbType.Int32);
            parameters.Add("AppointmentId", appointmentId, DbType.Int32);

            var query = "CALL mark_master_appointment_completed(@MasterId, @AppointmentId);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Appointment is marked as completed");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001"))
                {
                    if (ex.MessageText.Equals("Appointment is not found"))
                    {
                        return new Result<string>(new NotFoundException(ex.MessageText));
                    }
                    return new Result<string>(new ConflictException(ex.MessageText));
                }
            }
        }

        public async Task<Result<int>> GetCompletedAppointmentsCount(int masterId, DateRangeDto dateRange)
        {
            var parameters = new
            {
                Id = masterId,
                StartDate = dateRange.StartDate,
                EndDate = dateRange.EndDate
            };

            var query = "SELECT COUNT(*) AS completed_services "
                        + "FROM Appointment "
                        + "WHERE status = 'Completed' "
                        + "AND employee_id = @Id "
                        + "AND date BETWEEN @StartDate AND @EndDate;";

            using (var connection = _connectionService.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, parameters);
                return new Result<int>(count);
            }
        }

        public async Task<Result<double>> GetCompletedAppointmentsIncome(int masterId, DateRangeDto dateRange)
        {
            var parameters = new
            {
                Id = masterId,
                StartDate = dateRange.StartDate,
                EndDate = dateRange.EndDate
            };

            var query = "SELECT SUM(s.price) AS income "
                        + "FROM Appointment a "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "WHERE a.status = 'Completed' "
                        + "AND a.employee_id = @Id "
                        + "AND a.date BETWEEN @StartDate AND @EndDate;";

            using (var connection = _connectionService.CreateConnection())
            {
                var income = await connection.ExecuteScalarAsync<double>(query, parameters);
                return new Result<double>(income);
            }
        }

        public async Task<Result<IEnumerable<ManagerAppointmentDto>>> GetManagerAppointments(int salonId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT a.id, a.date, a.status, a.price, "
                        + "c.id, c.name, "
                        + "s.id, s.name, "
                        + "e.id, e.name, e.last_name "
                        + "FROM Appointment a "
                        + "JOIN Customer c ON a.customer_id = c.id "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId "
                        + "ORDER BY a.date "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    query, (ManagerAppointmentDto appointment,
                            CustomerDto customer,
                            ServiceNameDto service,
                            MasterDto master) =>
                    {
                        appointment.Master = master;
                        appointment.Service = service;
                        appointment.Customer = customer;
                        return appointment;
                    }, param: parameters
                );
                return new Result<IEnumerable<ManagerAppointmentDto>>(appointments);
            }
        }

        public async Task<Result<IEnumerable<AppointmentWithoutStatus>>> GetManagerActiveAppointments(int salonId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT a.id, a.date, a.price, "
                        + "c.id, c.name, "
                        + "s.id, s.name, "
                        + "e.id, e.name, e.last_name "
                        + "FROM Appointment a "
                        + "JOIN Customer c ON a.customer_id = c.id "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND a.status = 'Active' "
                        + "ORDER BY a.date "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    query, (AppointmentWithoutStatus appointment,
                            CustomerDto customer,
                            ServiceNameDto service,
                            MasterDto master) =>
                    {
                        appointment.Master = master;
                        appointment.Service = service;
                        appointment.Customer = customer;
                        return appointment;
                    }, param: parameters
                );
                return new Result<IEnumerable<AppointmentWithoutStatus>>(appointments);
            }
        }

        public async Task<Result<IEnumerable<AppointmentWithoutStatus>>> GetManagerCompletedAppointments(int salonId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT a.id, a.date, a.price, "
                        + "c.id, c.name, "
                        + "s.id, s.name, "
                        + "e.id, e.name, e.last_name "
                        + "FROM Appointment a "
                        + "JOIN Customer c ON a.customer_id = c.id "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND a.status = 'Completed' "
                        + "ORDER BY a.date "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    query, (AppointmentWithoutStatus appointment,
                            CustomerDto customer,
                            ServiceNameDto service,
                            MasterDto master) =>
                    {
                        appointment.Master = master;
                        appointment.Service = service;
                        appointment.Customer = customer;
                        return appointment;
                    }, param: parameters
                );
                return new Result<IEnumerable<AppointmentWithoutStatus>>(appointments);
            }
        }

        public async Task<Result<IEnumerable<AppointmentWithoutStatus>>> GetManagerUncompletedAppointments(int salonId, Paging paging)
        {
            var parameters = new
            {
                SalonId = salonId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT a.id, a.date, a.price, "
                        + "c.id, c.name, "
                        + "s.id, s.name, "
                        + "e.id, e.name, e.last_name "
                        + "FROM Appointment a "
                        + "JOIN Customer c ON a.customer_id = c.id "
                        + "JOIN Service s ON a.service_id = s.id "
                        + "JOIN Employee e ON a.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND a.status = 'Active' "
                        + "AND a.date <= CURRENT_TIMESTAMP "
                        + "ORDER BY a.date "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var appointments = await connection.QueryAsync(
                    query, (AppointmentWithoutStatus appointment,
                            CustomerDto customer,
                            ServiceNameDto service,
                            MasterDto master) =>
                    {
                        appointment.Master = master;
                        appointment.Service = service;
                        appointment.Customer = customer;
                        return appointment;
                    }, param: parameters
                );
                return new Result<IEnumerable<AppointmentWithoutStatus>>(appointments);
            }
        }
    }
}
