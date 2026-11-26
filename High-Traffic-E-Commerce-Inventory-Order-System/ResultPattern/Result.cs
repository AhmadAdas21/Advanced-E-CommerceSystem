namespace High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict
    }

    public sealed record AppError( ErrorType Type,string Code,string Message );

    public sealed class Result<T> where T : notnull
    {
        private readonly T? _value;
        private readonly AppError? _error;

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access Value of a failed result.");

        public AppError Error => IsFailure
            ? _error!
            : throw new InvalidOperationException("Cannot access Error of a successful result.");

        private Result(T value)
        {
            IsSuccess = true;
            _value = value;
        }

        private Result(AppError error)
        {
            IsSuccess = false;
            _error = error;
        }

        public static Result<T> Success(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Result<T>(value);
        }

        public static Result<T> Failure(AppError error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new Result<T>(error);
        }
    }
}
