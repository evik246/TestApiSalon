using Dapper;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.CityService
{
    public class CityService : ICityService
    {
        private readonly IDbConnectionService _connectionService;

        public CityService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Result<IEnumerable<City>>> GetAllCities(Paging paging)
        {
            var parameters = new
            {
                Skip = paging.Skip,
                Take = paging.PageSize
            };

            var query = "SELECT c.id, c.name "
                        + "FROM City c "
                        + "ORDER BY c.id "
                        + "OFFSET @Skip LIMIT @Take;";

            using (var connection = _connectionService.CreateConnection())
            {
                var cities = await connection.QueryAsync<City>(query, parameters);
                return new Result<IEnumerable<City>>(cities);
            }
        }
    }
}
