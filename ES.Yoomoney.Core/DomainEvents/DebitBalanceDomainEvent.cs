using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Enums;
using MediatR;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.DomainEvents
{
    public sealed record DebitBalanceDomainEvent(
        Guid StreamId, 
        decimal Amount,
        PaymentSystemsEnum paymentSystem,
        Payment paymentMeta): IDomainEvent, INotification;
}
