using ES.Yoomoney.Common.Core.Exceptions;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;

using MediatR;

namespace ES.Yoomoney.Application.Features.Queries;

public static class GetInvoicesQuery
{
    public sealed record Request(Guid AccountId, int Page = 1, int PageSize = 10) : IRequest<Response>;

    public sealed record Response(IReadOnlyCollection<DomainEvents.MoneyDepositedDomainEvent> Invoices, int TotalCount);

    public sealed class Handler(IEsEventStore eventStore) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var bankAccount = await eventStore.LoadAsync<BankAccountAggregate>(
                request.AccountId,
                version: null,
                cancellationToken);

            if (bankAccount == null)
            {
                throw new DomainException("Bank account not found");
            }

            var events = bankAccount
                .GetCommittedEvents()
                .OfType<DomainEvents.MoneyDepositedDomainEvent>()
                .ToArray();

            var paymentEvents = events
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new Response(paymentEvents, events.Length);
        }
    }
}