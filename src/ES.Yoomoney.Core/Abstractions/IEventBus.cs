namespace ES.Yoomoney.Core.Abstractions;

public interface IEventBus<in TIntegrationEvent> where TIntegrationEvent : IIntegrationEvent
{
    Task ProduceAsync(TIntegrationEvent @event, CancellationToken ct);
}