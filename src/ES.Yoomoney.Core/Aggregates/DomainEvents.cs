using ES.Yoomoney.Core.Abstractions;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Aggregates;

public static class DomainEvents
{
    public abstract record Event(Guid EventId, Guid StreamId) : IDomainEvent
    {
        public DateTime OccuredAt { get; init; } = DateTime.UtcNow;
    }

    public sealed record AccountBalanceInitializedTo(Guid AccountId)
        : Event(AccountId, AccountId);

    public sealed record InvoiceCreatedDomainEvent(
        Guid InvoiceId,
        Guid AccountId,
        decimal Amount) : Event(InvoiceId, AccountId);

    public sealed record MoneyDepositedDomainEvent(
        Guid InvoiceId,
        Guid AccountId,
        decimal Amount,
        string Currency,
        Payment Meta) : Event(InvoiceId, AccountId);

    public sealed record MoneyWithdrawnDomainEvent(
        Guid OperationId,
        Guid AccountId,
        decimal Amount) : Event(OperationId, AccountId);
}