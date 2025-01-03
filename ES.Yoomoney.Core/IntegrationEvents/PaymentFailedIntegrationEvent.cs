using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.IntegrationEvents;

public sealed record PaymentFailedIntegrationEvent(Guid OrderId, string ErrorMessage): IIntegrationEvent;