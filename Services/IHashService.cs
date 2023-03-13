namespace TestApiSalon.Services
{
    public interface IHashService
    {
        string Hash(string key);
        bool Verify(string hash, string key);
    }
}
