using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Countries.GetStates;

public sealed record GetStatesByCountryIdQuery(Guid CountryId) : IQuery<IReadOnlyList<StateResponse>>;
