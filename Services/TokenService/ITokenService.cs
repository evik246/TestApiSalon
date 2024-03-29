﻿using System.Security.Claims;

namespace TestApiSalon.Services.TokenService
{
    public interface ITokenService
    {
        string CreateToken(ClaimsIdentity identity);
        bool ValidateToken(string token);
        string? GetClaim(string token, string claimType);
        ClaimsIdentity? GetIdentity(string token);
    }
}
