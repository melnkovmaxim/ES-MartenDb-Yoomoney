using ES.Yoomoney.Common.Core.OperationResult;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Abstractions;

internal interface ICommandHandler : IRequestHandler<ICommand, Result>;

internal interface ICommandHandler<TResponse> : IRequestHandler<ICommand<TResponse>, Result<TResponse>>;