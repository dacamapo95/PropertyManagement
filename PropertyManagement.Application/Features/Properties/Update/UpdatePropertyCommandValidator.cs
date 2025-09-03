using FluentValidation;

namespace PropertyManagement.Application.Features.Properties.Update;

public sealed class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(400);
        RuleFor(x => x.CodeInternal).GreaterThan(0);
        RuleFor(x => x.Year).InclusiveBetween(1800, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.StateId).NotEmpty();
        RuleFor(x => x.CityId).NotEmpty();
        RuleFor(x => x.StatusId).GreaterThan(0);

        RuleFor(x => x.Owner).NotNull();
        RuleFor(x => x.Owner.IdentificationTypeId).GreaterThan(0);
        RuleFor(x => x.Owner.IdentificationNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Owner.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Owner.Address).MaximumLength(400);

        RuleForEach(x => x.PropertyFileIds).NotEmpty();
        RuleForEach(x => x.Owner.OwnerFileIds).NotEmpty();

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price!.Value).GreaterThan(0);
            RuleFor(x => x.PriceDate).NotNull();
        });
    }
}
