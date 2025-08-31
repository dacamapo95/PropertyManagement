using MediatR;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Core.Abstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;