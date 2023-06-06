using Dapper;
using Npgsql;
using System.Data;
using TestApiSalon.Dtos.Appointment;
using TestApiSalon.Dtos.Employee;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Schedule;
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

        public async Task<Result<string>> ChangeManagerMasterSchedule(int salonId, int scheduleId, MasterScheduleChangeDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("SalonId", salonId, DbType.Int32);
            parameters.Add("ScheduleId", scheduleId, DbType.Int32);
            parameters.Add("Weekday", request.Weekday?.ToString(), DbType.AnsiStringFixedLength);
            parameters.Add("StartTime", request.StartTime, DbType.Time);
            parameters.Add("EndTime", request.EndTime, DbType.Time);

            var query = "CALL update_master_schedule(@SalonId, @ScheduleId, @Weekday, @StartTime, @EndTime);";

            using (var connection =  _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Master schedule is changed successfully");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23514"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("check_time"))
                        {
                            return new Result<string>(new ConflictException("The number of working hours exceeds the maximum limit"));
                        }
                    }
                    return new Result<string>(new ConflictException(ex.Message));
                }
            }
        }

        public async Task<Result<string>> DeleteManagerMasterSchedule(int salonId, int scheduleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("SalonId", salonId, DbType.Int32);
            parameters.Add("ScheduleId", scheduleId, DbType.Int32);

            var query = "CALL delete_master_schedule(@SalonId, @ScheduleId);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Master schedule is deleted successfully");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001"))
                {
                    if (ex.MessageText.Contains("master does not work at this salon"))
                    {
                        return new Result<string>(new ConflictException("Master does not work at this salon"));
                    }
                    return new Result<string>(ex.MessageText);
                }
            }
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

        public async Task<Result<IEnumerable<Schedule>>> GetManagerMasterSchedule(int salonId, int masterId)
        {
            var parameters = new
            {
                SalonId = salonId,
                MasterId = masterId
            };

            var query = "SELECT s.id, s.weekday, s.start_time, s.end_time "
                        + "FROM Schedule s "
                        + "JOIN Employee e ON s.employee_id = e.id "
                        + "WHERE e.salon_id = @SalonId AND s.employee_id = @MasterId;";

            using (var connection = _connectionService.CreateConnection())
            {
                var schedule = await connection.QueryAsync<Schedule>(query, parameters);
                return new Result<IEnumerable<Schedule>>(schedule);
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

        public async Task<Result<string>> SetManagerMasterSchedule(int salonId, int masterId, MasterScheduleCreateDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("SalonId", salonId, DbType.Int32);
            parameters.Add("MasterId", masterId, DbType.Int32);
            parameters.Add("Weekday", request.Weekday.ToString(), DbType.AnsiStringFixedLength);
            parameters.Add("StartTime", request.StartTime, DbType.Time);
            parameters.Add("EndTime", request.EndTime, DbType.Time);

            var query = "CALL set_master_schedule(@SalonId, @MasterId, @Weekday, @StartTime, @EndTime);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Master schedule is created successfully");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23514"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("check_time"))
                        {
                            return new Result<string>(new ConflictException("The number of working hours exceeds the maximum limit"));
                        }
                    }
                    return new Result<string>(new ConflictException(ex.Message));
                }
            }
        }
    }
}
