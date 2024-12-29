using ES.Yoomoney.Core.DomainEvents;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Projections;

public sealed record BalanceProjection() : IApplicationProjection
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public long Version { get; init; }
    public int EventsCount { get; init; }

    public BalanceProjection(Payment payment) : this()
    {
        if (payment.Paid == false)
        {
            throw new Exception("Is not paid");
        }

        AccountId = FetchPaymentsWorker.UserId;
        Amount = payment.Amount.Value;
    }

    public BalanceProjection Apply(DebitBalanceDomainEvent @event)
    {
        return this with
        {
            Amount = Amount + @event.Amount,
            EventsCount = EventsCount + 1,
            Version = Version + 1
        };
    }

    public BalanceProjection Apply(TopupBalanceDomainEvent @event)
    {
        return this with
        {
            Amount = Amount - @event.Amount,
            EventsCount = EventsCount + 1,
            Version = Version + 1
        };
    }
}