﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestApiSalon.Models;

namespace TestApiSalon.Services
{
    public class JsonTokenGeneratorService : ITokenGeneratorService<Customer>
    {
        private readonly IConfiguration _configuration;

        public JsonTokenGeneratorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(Customer request)
        {
            if (request is null)
            {
                throw new ArgumentNullException("Invalid customer");
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Sid, request.Id.ToString()),
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Name, request.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}