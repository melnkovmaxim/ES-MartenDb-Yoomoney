using ES.Yoomoney.Common.Core.OperationResult;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Abstractions;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;