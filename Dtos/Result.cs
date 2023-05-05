namespace TestApiSalon.Dtos
{
    public class Result<T>
    {
        public ResultState State { get; }

        public readonly T? Value;

        public readonly Exception? Exception;

        public Result(T value)
        {
            State = ResultState.Success;
            Value = value;
        }

        public Result(Exception exception)
        {
            State = ResultState.Failure;
            Exception = exception;
        }

        public TResult Match<TResult>(Func<T, TResult> success, Func<Exception, TResult> failure) 
        {
            return State switch
            {
                ResultState.Success => success(Value!),
                ResultState.Failure => failure(Exception!),
                _ => throw new InvalidOperationException()
            };
        }
    }

    public enum ResultState
    {
        Success,
        Failure
    }
}
