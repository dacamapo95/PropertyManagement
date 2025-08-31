﻿namespace PropertyManagement.Shared.Results;

public class ValidationResult : Result, IValidationResult
{
    private ValidationResult(Error[] errors)
        : base(false, IValidationResult.Error) => Errors = errors;

    public Error[] Errors { get; }

    public static ValidationResult WithErrors(Error[] errors) => new(errors);
}

public class ValidationResult<T> : Result<T>, IValidationResult
{
    private ValidationResult(Error[] errors) : base(false, default, IValidationResult.Error)
    {
        Errors = errors;
    }

    public static ValidationResult<T> WithErrors(Error[] errors) => new(errors);

    public Error[] Errors { get; }
}