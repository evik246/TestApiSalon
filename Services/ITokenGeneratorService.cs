using TestApiSalon.Dtos;

namespace TestApiSalon.Services
{
    public interface ITokenGeneratorService<T> where T : class
    {
        string CreateToken(T request);
    }
}
