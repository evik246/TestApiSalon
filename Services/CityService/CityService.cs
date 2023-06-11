using Dapper;
using TestApiSalon.Dtos.Cities;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Exceptions;
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

        public async Task<Result<string>> CreateCity(CityDto request)
        {
            var parameters = new
            {
                Name = request.Name
            };

            var query = "INSERT INTO City (name) VALUES (@Name);";

            using (var connection = _connectionService.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return new Result<string>("City is created");
            }
        }

        public async Task<Result<string>> UpdateCity(int cityId, CityDto request)
        {
            var parameters = new
            {
                CityId = cityId,
                Name = request.Name
            };

            var query = "UPDATE City SET name = @Name WHERE id = @CityId;";

            using(var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("City is not found"));
                }
                return new Result<string>("City is changed");
            }
        }

        public async Task<Result<string>> DeleteCity(int cityId)
        {
            var parameters = new
            {
                CityId = cityId
            };

            var query = "DELETE FROM City WHERE id = @CityId;";

            using (var connection = _connectionService.CreateConnection())
            {
                int rows = await connection.ExecuteAsync(query, parameters);
                if (rows == 0)
                {
                    return new Result<string>(new NotFoundException("City is not found"));
                }
                return new Result<string>("City is deleted");
            }
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
