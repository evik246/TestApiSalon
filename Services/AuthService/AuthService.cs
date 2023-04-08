using Dapper;
using Npgsql;
using System.Data;
using TestApiSalon.Data;
using TestApiSalon.Dtos;
using TestApiSalon.Models;
using TestApiSalon.Services.ClaimsIdentityService;
using TestApiSalon.Services.ConnectionService;
using TestApiSalon.Services.TokenService;

namespace TestApiSalon.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnectionService _connectionService;
        private readonly ITokenService _tokenService;
        private readonly IClaimsIdentityService<User> _identityService;

        public AuthService(IDbConnectionService connectionService, ITokenService tokenService,
            IClaimsIdentityService<User> identityService)
        {
            _connectionService = connectionService;
            _tokenService = tokenService;
            _identityService = identityService;
        }

        public async Task<string?> Login(UserLoginDto request)
        {
            _connectionService.ConnectionName = DbConnectionName.Default;

            var parameters = new
            {
                Email = request.Email,
                Password = request.Password
            };

            var query = "SELECT * FROM salon_users_schema.Users " +
                "WHERE email = @Email AND password = crypt(@Password, password);";

            User? user;
            using (var connection = _connectionService.CreateConnection())
            {
                user = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);

                if (user == null) { return null; }
            }
            return _tokenService.CreateToken(_identityService.CreateClaimsIdentity(user));
        }

        public async Task<bool> ResetPassword(UserUpdatePasswordDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", request.Email, DbType.AnsiStringFixedLength);
            parameters.Add("OldPassword", request.CurrentPassword, DbType.AnsiStringFixedLength);
            parameters.Add("NewPassword", request.NewPassword, DbType.AnsiStringFixedLength);

            var query = "CALL change_password(@Email, @OldPassword, @NewPassword);";

            using (var connection = _connectionService.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return true;
                }
                catch (PostgresException ex) when (ex.SqlState.Equals("P0001")) 
                {
                    return false;
                }
            }
        }
    }
}
