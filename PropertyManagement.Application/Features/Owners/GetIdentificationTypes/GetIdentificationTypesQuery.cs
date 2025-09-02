using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Owners.GetIdentificationTypes;

public sealed record GetIdentificationTypesQuery() : IQuery<IReadOnlyList<IdentificationTypeResponse>>;
