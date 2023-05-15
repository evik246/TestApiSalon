using Dapper;
using TestApiSalon.Dtos.Other;
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
    }
}
