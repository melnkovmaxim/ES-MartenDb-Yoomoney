using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Enums;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Aggregates;

public static class Events
{
    public record Event(Guid StreamId) : IDomainEvent
    {
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }

    public sealed record AccountBalanceInitializedTo(Guid AccountId) : Event(AccountId);

    public sealed record DebitBalanceDomainEvent(
        Guid AccountId,
        decimal Amount,
        PaymentSystemsEnum paymentSystem,
        Payment paymentMeta) : Event(AccountId);

    public sealed record TopupBalanceDomainEvent(
        Guid AccountId, 
        decimal Amount) : Event(AccountId);
}