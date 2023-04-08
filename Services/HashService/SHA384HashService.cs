namespace TestApiSalon.Services.HashService
{
    public class SHA384HashService : IHashService
    {
        public string Hash(string key, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(key, salt);
        }

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
