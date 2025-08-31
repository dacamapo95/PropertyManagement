namespace PropertyManagement.Shared.Results;

public interface IValidationResult
{
    public static readonly Error Error =
        new("Validation.Error", "A validation problem ocurred");

    Error[] Errors { get; }
}
