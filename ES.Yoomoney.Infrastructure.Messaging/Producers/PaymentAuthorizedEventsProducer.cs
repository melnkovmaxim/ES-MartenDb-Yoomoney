using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;

namespace ES.Yoomoney.Infrastructure.Messaging.Producers;

internal sealed class PaymentAuthorizedEventsProducer(
    IMessageProducer<InvoiceStatusChangedIntegrationEvent> producer): IEventBus<InvoiceStatusChangedIntegrationEvent>
{
    public Task ProduceAsync(InvoiceStatusChangedIntegrationEvent @event, CancellationToken ct)
    {
        return producer.ProduceAsync(@event.Invoice.MerchantCustomerId, @event);
    }
}