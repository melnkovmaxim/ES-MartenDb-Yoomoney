using ES.Yoomoney.Common.Core.FunctionalResult;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;