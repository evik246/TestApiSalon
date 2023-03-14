﻿namespace TestApiSalon.Services
{
    public class SHA384HashService : IHashService
    {
        public string Hash(string key)
        {
            return BCrypt.Net.BCrypt.HashPassword(key);
        }

        public bool Verify(string hash, string key)
        {
            return BCrypt.Net.BCrypt.Verify(key, hash);
        }
    }
}