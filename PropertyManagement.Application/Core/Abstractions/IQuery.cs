using MediatR;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;