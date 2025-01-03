using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.IntegrationEvents;

public sealed record OrderCreatedIntegrationEvent(Guid OrderId, decimal Price): IIntegrationEvent;