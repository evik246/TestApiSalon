using System.Globalization;

namespace TestApiSalon.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base() { }

        public ConflictException(string message) : base(message) { }

        public ConflictException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
