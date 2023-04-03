using Dapper;
using TestApiSalon.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Models;
using TestApiSalon.Services.ClaimsIdentityService;
using TestApiSalon.Services.ConnectionService;
using TestApiSalon.Services.HashService;
using TestApiSalon.Services.TokenService;

namespace TestApiSalon.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnectionService _connectionService;
        private readonly ITokenService _tokenService;
        private readonly IClaimsIdentityService<User> _identityService;
        private readonly IHashService _hashService;

        public AuthService(IDbConnectionService connectionService, ITokenService tokenService,
            IClaimsIdentityService<User> identityService, IHashService hashService)
        {
            _connectionService = connectionService;
            _tokenService = tokenService;
            _identityService = identityService;
            _hashService = hashService;

            _connectionService.ConnectionName = DbConnectionName.Default;
        }

        public async Task<string?> Login(UserLoginDto request)
        {
            var parameters = new
            {
                Email = request.Email
            };

            var query = "SELECT * FROM salon_users_schema.Users WHERE EMAIL = @Email";

            User? user = null;

            using (var connection = _connectionService.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query, parameters);
                if (users.Any())
                {
                    user = users.First();
                }
            }

            if (user is null || _hashService.Verify(user.Password.Trim(), user.Password) == false) 
            {
                return null;
            }

            return _tokenService.CreateToken(_identityService.CreateClaimsIdentity(user));
        }
    }
}
