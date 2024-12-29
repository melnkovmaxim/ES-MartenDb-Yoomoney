using ES.Yoomoney.Core.DomainEvents;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Projections;

public sealed record BalanceProjection() : IApplicationProjection
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public long Version { get; init; }
    public int EventsCount { get; init; }

    public BalanceProjection(AccountBalanceInitializedEvent @event) : this()
    {
        AccountId = @event.StreamId;
        Version = 1;
        EventsCount = 1;
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
    
    public BalanceProjection Apply(AccountBalanceInitializedEvent @event)
    {
        return this;
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