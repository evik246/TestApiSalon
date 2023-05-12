using Dapper;
using TestApiSalon.Dtos;
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

        public async Task<Result<IEnumerable<CustomerAppointmentDto>>> GetCustomerAppointments(int customerId, int salonId, Paging paging)
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
    }
}
