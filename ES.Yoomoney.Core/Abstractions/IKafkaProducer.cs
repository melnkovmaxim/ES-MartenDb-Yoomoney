using ES.Yoomoney.Core.IntegrationEvents;

namespace ES.Yoomoney.Core.Abstractions;

public interface IKafkaProducer<in TIntegrationEvent> where TIntegrationEvent: IIntegrationEvent
{
    Task ProduceAsync(TIntegrationEvent @event, CancellationToken ct);
}