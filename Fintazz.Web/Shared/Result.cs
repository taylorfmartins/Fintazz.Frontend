namespace Fintazz.Web.Shared;

public class Result
{
    public bool IsSuccess { get; }
    public ApiError? Error { get; }

    protected Result(bool isSuccess, ApiError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(ApiError error) => new(false, error);
    public static Result Failure(string message) => new(false, new ApiError("Error", message));
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null) => Value = value;
    private Result(ApiError error) : base(false, error) { }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(ApiError error) => new(error);
    public new static Result<T> Failure(string message) => new(new ApiError("Error", message));
}
