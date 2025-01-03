using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.IntegrationEvents;

public sealed record PaymentAuthorizedIntegrationEvent(Guid OrderId): IIntegrationEvent;