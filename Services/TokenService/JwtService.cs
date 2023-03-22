using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestApiSalon.Services.TokenService
{
    public class JwtService : ITokenService
    {
        private readonly IConfiguration _configuration;

        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;

            _key = _configuration.GetSection("AppSettings:Key").Value ?? string.Empty;
            _issuer = _configuration.GetSection("AppSettings:Issuer").Value ?? string.Empty;
            _audience = _configuration.GetSection("AppSettings:Audience").Value ?? string.Empty;

            if (string.IsNullOrEmpty(_key) ||
                string.IsNullOrEmpty(_issuer) ||
                string.IsNullOrEmpty(_audience))
            {
                throw new Exception("Invalid configuration settings for JWT token");
            }
        }

        public string CreateToken(ClaimsIdentity identity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = securityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string? GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken securityToken) { return null; }

            var stringClaimValue = securityToken.Claims.First(
                claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }

        public ClaimsIdentity? GetIdentity(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken securityToken) { return null; }

            var claimsIdentity = new ClaimsIdentity(securityToken.Claims, "CustomJwt");
            return claimsIdentity;
        }
    }
}
