using PropertyManagement.Application.Core.Abstractions;

namespace PropertyManagement.Application.Features.Properties.GetById;

public sealed record GetPropertyByIdQuery(Guid Id) : IQuery<PropertyResponse>;
