using ES.Yoomoney.Core.Abstractions;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.IntegrationEvents;

public sealed record PaymentReceivedIntegrationEvent(IReadOnlyCollection<Payment> paidPayments): IIntegrationEvent;