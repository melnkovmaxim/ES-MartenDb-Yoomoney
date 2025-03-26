using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Entities;
using ES.Yoomoney.Core.Aggregates;
using MediatR;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Application.Features.Queries;

public static class GetInvoicesQuery
{
    public sealed record Request(Guid AccountId, int Page = 1, int PageSize = 10) : IRequest<Response>;

    public sealed record Response(IReadOnlyCollection<DomainEvents.MoneyDepositedDomainEvent> Invoices, int TotalCount);

    public sealed class Handler(IEsEventStore eventStore) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var bankAccount = await eventStore.LoadAsync<BankAccountAggregate>(request.AccountId, version: null, cancellationToken);
            
            if (bankAccount == null)
            {
                return new Response([], 0);
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