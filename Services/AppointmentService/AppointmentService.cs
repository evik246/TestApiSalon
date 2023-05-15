using Dapper;
using Npgsql;
using System.Data;
using System.Text;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Service;
using TestApiSalon.Exceptions;
using TestApiSalon.Extensions;
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
                        + "OFFSET 0 LIMIT 50;";

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

        public async Task<Result<string>> CreateAppointment(AppointmentCreateDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Date", request.Date.ToCorrectDateTime(), DbType.DateTime2);
            parameters.Add("CustomerId", request.CustomerId, DbType.Int32);
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

        public async Task<Result<string>> MarkAppointmentCompleted(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);

            var query = "CALL mark_appointment_completed(@Id);";

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
    }
}
