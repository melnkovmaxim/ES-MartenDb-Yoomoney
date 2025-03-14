using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Projections;

namespace ES.Yoomoney.Core.Aggregates;

public sealed partial class BankAccountAggregate
{
    public BankAccountAggregate(Events.AccountBalanceInitializedTo to)
    {
        Events.Add(to);
    }

    public BankAccountAggregate Apply(Events.AccountBalanceInitializedTo to)
    {
        return new BankAccountAggregate(to);
    }

    public BankAccountAggregate Apply(Events.DebitBalanceDomainEvent @event)
    {
        Balance += @event.Amount;
        Events.Add(@event);

        return this;
    }

    public BankAccountAggregate Apply(Events.TopupBalanceDomainEvent @event)
    {
        Balance -= @event.Amount;
        Events.Add(@event);

        return this;
    }
}