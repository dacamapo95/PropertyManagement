namespace PropertyManagement.Shared.Results;

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => !IsValid ? throw new InvalidOperationException("No value for failure.") : _value!;

    protected internal Result(bool isValid, T value, Error error)
        : base(isValid, error)
    {
        _value = value;
    }

    public static implicit operator Result<T>(T value) =>
        value is not null ? Success(value) : Fail<T>(Error.None);

    public static implicit operator Result<T>(Error error) => Fail<T>(error);
}