using FluentValidation;

namespace PropertyManagement.Application.Features.Properties.Create;

public sealed class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(400).WithMessage("Address must not exceed 400 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Tax)
            .GreaterThanOrEqualTo(0).WithMessage("Tax must be greater than or equal to 0.");

        RuleFor(x => x.CodeInternal)
            .GreaterThan(0).WithMessage("Internal code must be greater than 0.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1800, DateTime.UtcNow.Year + 1)
            .WithMessage("Year must be between 1800 and current year + 1.");

        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("Status is invalid.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.StateId)
            .NotEmpty().WithMessage("State is required.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.Owner)
            .NotNull().WithMessage("Owner data is required.");

        RuleFor(x => x.Owner.IdentificationTypeId)
            .GreaterThan(0).WithMessage("Identification type is invalid.");

        RuleFor(x => x.Owner.IdentificationNumber)
            .NotEmpty().WithMessage("Identification number is required.")
            .MaximumLength(100).WithMessage("Identification number must not exceed 100 characters.");

        RuleFor(x => x.Owner.Name)
            .NotEmpty().WithMessage("Owner name is required.")
            .MaximumLength(200).WithMessage("Owner name must not exceed 200 characters.");

        RuleFor(x => x.Owner.Address)
            .MaximumLength(400).WithMessage("Owner address must not exceed 400 characters.");

        RuleForEach(x => x.PropertyFileIds)
            .NotEmpty().WithMessage("Invalid property file id.");

        RuleForEach(x => x.Owner.OwnerFileIds)
            .NotEmpty().WithMessage("Invalid owner file id.");
    }
}
