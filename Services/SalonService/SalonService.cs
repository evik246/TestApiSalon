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
                        + "ORDER BY s.id "
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
