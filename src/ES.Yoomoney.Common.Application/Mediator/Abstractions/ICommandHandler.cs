using ES.Yoomoney.Common.Core.FunctionalResult;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Abstractions;

internal interface ICommandHandler : IRequestHandler<ICommand, Result>;

internal interface ICommandHandler<TResponse> : IRequestHandler<ICommand<TResponse>, Result<TResponse>>;