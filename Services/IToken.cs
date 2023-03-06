using TestApiSalon.Dtos;

namespace TestApiSalon.Services
{
    public interface IToken<T> where T : class
    {
        string Create(T request);
    }
}
