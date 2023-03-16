namespace TestApiSalon.Services
{
    public interface ITokenGeneratorService<T>
    {
        string CreateToken(T request);
    }
}
