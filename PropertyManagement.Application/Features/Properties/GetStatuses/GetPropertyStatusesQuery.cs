using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Properties.GetStatuses;

public sealed record GetPropertyStatusesQuery() : IQuery<IReadOnlyList<PropertyStatusResponse>>;
