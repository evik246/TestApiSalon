namespace TestApiSalon.Services.HashService
{
    public interface IHashService
    {
        string Hash(string key, string salt);
        string Hash(string key);
        bool Verify(string hash, string key);
    }
}
