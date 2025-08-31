namespace PropertyManagement.Shared.Results;

public class Result
{
    public bool IsValid { get; }

    public bool IsFailure => !IsValid;

    public Error Error { get; }

    protected internal Result(bool isValid, Error error)
    {
        if (isValid && error != Error.None)
        {
            throw new ArgumentException("Invalid result instance", nameof(error));
        }

        IsValid = isValid;
        Error = error;
    }

    public static Result Success() => new Result(true, Error.None);

    public static Result<T> Success<T>(T value) => new(true, value, Error.None);

    public static Result Fail(Error error) => new(false, error);

    public static Result<T> Fail<T>(Error error) => new(false, value: default, error: error);

    public static implicit operator Result(Error error) => Fail(error);

}