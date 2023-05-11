using Dapper;
using System.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly IDbConnectionService _connectionService;

        public ScheduleService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots(CustomerAppointmentDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("EmployeeId", request.MasterId, DbType.Int32);
            parameters.Add("ServiceId", request.ServiceId, DbType.Int32);
            parameters.Add("Date", request.Date, DbType.Date);

            var query = "SELECT * FROM get_available_time_slots(@EmployeeId, @ServiceId, @Date);";

            using (var connection = _connectionService.CreateConnection())
            {
                var timeslots = await connection.QueryAsync<AvailableTimeSlotDto>(query, parameters);
                return new Result<IEnumerable<AvailableTimeSlotDto>>(timeslots);
            }
        }
    }
}
