using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Projections;

namespace ES.Yoomoney.Core.Aggregates;

public sealed partial class BankAccountAggregate
{
    private BankAccountAggregate Apply(Events.AccountBalanceInitializedTo @event)
    {
        Id = @event.StreamId;
        
        IncreaseVersion();

        return this;
    }

    private BankAccountAggregate Apply(Events.MoneyDepositedDomainEvent @event)
    {
        Balance += @event.Amount;

        IncreaseVersion();

        return this;
    }

    private BankAccountAggregate Apply(Events.MoneyWithdrawnDomainEvent @event)
    {
        Balance -= @event.Amount;

        IncreaseVersion();

        return this;
    }
}