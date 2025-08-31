using MediatR;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Core.Abstractions;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
