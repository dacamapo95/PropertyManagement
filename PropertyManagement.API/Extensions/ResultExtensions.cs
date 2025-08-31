using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.API.Extensions;

public static class ResultExtension
{
    public static IResult ResultToResponse(Result result)
    {
        if (result.IsValid)
        {
            throw new InvalidOperationException("Result is valid");
        }

        return result switch
        {
            IValidationResult validationResult => CreateValidationProblem(validationResult, result),
            _ => Results.Problem(
                statusCode: GetStatusCode(result.Error.ErrorType),
                title: GenerateProblemTitle(result.Error.ErrorType),
                type: GetErrorTypeReference(result.Error.ErrorType),
                detail: result.Error.Message)
        };
    }

    private static ProblemDetails BuildValidationProblemDetails(
        string title,
        int status,
        Error error,
        Error[]? errors = null) =>
        new ProblemDetails()
        {
            Title = title,
            Type = GetErrorTypeReference(ErrorTypeEnum.Validation),
            Detail = error.Message,
            Status = status,
            Extensions =
           {
               {
                   nameof(errors),
                   errors?
                   .GroupBy(error => error.Code)
                   .ToDictionary(groupByProperty => groupByProperty.Key,
                                 errors => errors.Select(error => error.Message).ToArray())
               }
           }
        };

    private static int GetStatusCode(ErrorTypeEnum errorType) =>
        errorType switch
        {
            ErrorTypeEnum.BadRequest => StatusCodes.Status400BadRequest,
            ErrorTypeEnum.NotFound => StatusCodes.Status404NotFound,
            ErrorTypeEnum.Validation => StatusCodes.Status400BadRequest,
            ErrorTypeEnum.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorTypeEnum.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GenerateProblemTitle(ErrorTypeEnum errorType) =>
        errorType switch
        {
            ErrorTypeEnum.Validation => "Validation error",
            ErrorTypeEnum.NotFound => "Resource not found",
            ErrorTypeEnum.BadRequest => "Bad request",
            ErrorTypeEnum.Unauthorized => "Unauthorized",
            ErrorTypeEnum.Conflict => "Conflict",
            _ => "Internal server error",
        };

    private static string GetErrorTypeReference(ErrorTypeEnum errorType) =>
        errorType switch
        {
            ErrorTypeEnum.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorTypeEnum.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorTypeEnum.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorTypeEnum.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ErrorTypeEnum.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

    public static IResult CreateValidationProblem(IValidationResult validationResult, Result result) =>
         Results.Problem(BuildValidationProblemDetails(
            "Validation error ocurred.",
            StatusCodes.Status400BadRequest,
            result.Error,
            validationResult.Errors));
}