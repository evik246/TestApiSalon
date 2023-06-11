using Dapper;
using Npgsql;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Salon;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.SalonService
{
    public class SalonService : ISalonService
    {
        private readonly IDbConnectionService _connectionService;

        public SalonService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Result<string>> CreateSalon(SalonCreateDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Address", request.Address, DbType.AnsiStringFixedLength);
            parameters.Add("CityId", request.CityId, DbType.Int32);
            parameters.Add("Phone", request.Phone, DbType.AnsiStringFixedLength);

            var query = "INSERT INTO Salon(address, city_id, phone) VALUES (@Address, @CityId, @Phone);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return new Result<string>("Salon is created");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("salon_phone_key"))
                        {
                            return new Result<string>(new ConflictException("This phone is already used"));
                        }
                    }
                    return new Result<string>(new ConflictException("Invalid data"));
                }
            }
        }

        public async Task<Result<string>> UpdateSalon(int salonId, SalonChangeDto request)
        {
            var query = new StringBuilder("UPDATE Salon SET ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Address))
            {
                query.Append("address = @Address, ");
                parameters.Add("Address", request.Address, DbType.AnsiStringFixedLength);
            }

            if (request.CityId != null)
            {
                query.Append("city_id = @CityId, ");
                parameters.Add("CityId", request.CityId, DbType.Int32);
            }

            if (!string.IsNullOrEmpty(request.Phone))
            {
                query.Append("phone = @Phone, ");
                parameters.Add("Phone", request.Phone, DbType.AnsiStringFixedLength);
            }

            if (query.ToString().EndsWith(", "))
            {
                query.Remove(query.Length - 2, 2);
            }

            query.Append(" WHERE id = @Id;");
            parameters.Add("Id", salonId, DbType.Int32);

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    int rows = await connection.ExecuteAsync(query.ToString(), parameters);
                    if (rows == 0)
                    {
                        return new Result<string>(new NotFoundException("Salon is not found"));
                    }
                    return new Result<string>("Salon is changed");
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("23505"))
                {
                    if (!string.IsNullOrEmpty(ex.ConstraintName))
                    {
                        if (ex.ConstraintName.Equals("salon_phone_key"))
                        {
                            return new Result<string>(new ConflictException("This phone is already used"));
                        }
                    }
                    return new Result<string>(new ConflictException("Invalid data"));
                }
            }
        }

        public async Task<Result<string>> DeleteSalon(int salonId)
        {
            var parameters = new
            {
                SalonId = salonId
            };

            var query = "DELETE FROM Salon WHERE id = @SalonId;";

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("Salon is not found"));
                }
                return new Result<string>("Salon is created");
            }
        }

        public async Task<Result<IEnumerable<Salon>>> GetAllSalons(Paging paging)
        {
            var parameters = new
            {
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT s.id, s.address, s.phone, "
                        + "c.id, c.name "
                        + "FROM Salon s "
                        + "JOIN City c ON s.city_id = c.id "
                        + "ORDER BY s.address, c.name "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var salons = await connection.QueryAsync(
                    query, (Salon salon, City city) =>
                    {
                        salon.City = city;
                        salon.CityId = city.Id;
                        return salon;
                    }, param: parameters
                );
                return new Result<IEnumerable<Salon>>(salons);
            }
        }

        public async Task<Result<Salon>> GetSalonById(int salonId)
        {
            var parameters = new
            {
                SalonId = salonId
            };

            var query = "SELECT s.id, s.address, s.phone, "
                        + "c.id, c.name "
                        + "FROM Salon s "
                        + "JOIN City c ON s.city_id = c.id "
                        + "WHERE s.id = @SalonId;";

            using (var connection = _connectionService.CreateConnection())
            {
                var salons = await connection.QueryAsync(
                    query, (Salon salon, City city) =>
                    {
                        salon.City = city;
                        salon.CityId = city.Id;
                        return salon;
                    }, param: parameters
                );
                if (!salons.Any())
                {
                    return new Result<Salon>(new NotFoundException("Salon is not found"));
                }
                return new Result<Salon>(salons.First());
            }
        }

        public async Task<Result<IEnumerable<Salon>>> GetSalonsInCity(int cityId, Paging paging)
        {
            var parameters = new
            {
                Id = cityId,
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT s.id, s.address, s.phone, "
                        + "c.id, c.name "
                        + "FROM Salon s "
                        + "JOIN City c ON s.city_id = c.id "
                        + "WHERE c.id = @Id "
                        + "ORDER BY s.address "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var salons = await connection.QueryAsync(
                    query, (Salon salon, City city) =>
                    {
                        salon.City = city;
                        salon.CityId = city.Id;
                        return salon;
                    }, param: parameters
                );
                return new Result<IEnumerable<Salon>>(salons);
            }
        }

        public async Task<Result<SalonWithAddress>> GetSalonWithAddressById(int salonId)
        {
            var parameters = new
            {
                SalonId = salonId
            };

            var query = "SELECT s.id, s.address, "
                        + "c.id, c.name "
                        + "FROM Salon s "
                        + "JOIN City c ON s.city_id = c.id "
                        + "WHERE s.id = @SalonId;";

            using (var connection = _connectionService.CreateConnection())
            {
                var salons = await connection.QueryAsync(
                    query, (SalonWithAddress salon, City city) =>
                    {
                        salon.City = city;
                        return salon;
                    }, param: parameters
                );
                if (!salons.Any())
                {
                    return new Result<SalonWithAddress>(new NotFoundException("Salon is not found"));
                }
                return new Result<SalonWithAddress>(salons.First());
            }
        }
    }
}
