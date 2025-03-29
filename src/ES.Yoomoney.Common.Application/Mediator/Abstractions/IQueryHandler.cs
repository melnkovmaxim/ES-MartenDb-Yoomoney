using ES.Yoomoney.Common.Core.OperationResult;

using MediatR;

namespace ES.Yoomoney.Common.Application.Mediator.Abstractions;

internal interface IQueryHandler<TResponse> : IRequestHandler<IQuery<TResponse>, Result<TResponse>>;