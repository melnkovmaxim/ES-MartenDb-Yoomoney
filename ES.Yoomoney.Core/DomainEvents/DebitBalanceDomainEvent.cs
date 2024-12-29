using ES.Yoomoney.Core.Enums;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.DomainEvents
{
    public record DebitBalanceDomainEvent(
        Guid StreamId, 
        decimal Amount,
        PaymentSystemsEnum paymentSystem,
        Payment paymentMeta): IDomainEvent;
}
