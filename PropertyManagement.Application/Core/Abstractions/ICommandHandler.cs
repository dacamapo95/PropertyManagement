﻿using MediatR;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Core.Abstractions;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;