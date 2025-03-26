using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using MediatR;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Application.Features.Events;

public static class InvoiceStatusChangedEvent
{
    public sealed record Event(Payment Invoice): INotification;
    public sealed class Handler(IEsEventStore eventStore): INotificationHandler<Event>
    {
        public async Task Handle(Event request, CancellationToken ct)
        {
            var invoice = request.Invoice;
            var eventId = Guid.Parse(invoice.Id);

            if (await eventStore.ExistsAsync(eventId, ct))
            {
                return;
            }
            
            var accountId = Guid.Parse(invoice.MerchantCustomerId);
            var account = await eventStore.LoadAsync<BankAccountAggregate>(accountId, null, ct);

            if (account is null)
            {
                throw new Exception("Unknown account");
            }
            
            account.Deposit(eventId, invoice.Amount.Value, invoice.Amount.Currency, request.Invoice);

            await eventStore.StoreAsync(account, ct);
        }
    }
}