using Dapper;
using System.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
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

        public async Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots
            (CustomerAppointmentInputDto request)
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

        public async Task<Result<IEnumerable<Schedule>>> GetMasterSchedule(int employeeId)
        {
            var parameters = new
            {
                Id = employeeId
            };

            var query = "SELECT s.id, s.weekday, s.start_time, s.end_time "
                        + "FROM Schedule s "
                        + "WHERE s.employee_id = @Id;";

            using (var connection = _connectionService.CreateConnection())
            {
                var schedule = await connection.QueryAsync<Schedule>(query, parameters);
                return new Result<IEnumerable<Schedule>>(schedule);
            }
        }

        public async Task<Result<IEnumerable<MasterWorkingDayDto>>> GetMasterWorkingDays
            (int employeeId, DateRangeDto dateRange)
        {
            var parameters = new DynamicParameters();
            parameters.Add("EmployeeId", employeeId, DbType.Int32);
            parameters.Add("StartDate", dateRange.StartDate, DbType.Date);
            parameters.Add("EndDate", dateRange.EndDate, DbType.Date);

            var query = "SELECT * FROM get_master_working_days(@EmployeeId, @StartDate, @EndDate);";

            using (var connection = _connectionService.CreateConnection())
            {
                var days = await connection.QueryAsync<MasterWorkingDayDto>(query, parameters);
                return new Result<IEnumerable<MasterWorkingDayDto>>(days);
            }
        }
    }
}
