using ES.Yoomoney.Common.Core.OperationResult;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;