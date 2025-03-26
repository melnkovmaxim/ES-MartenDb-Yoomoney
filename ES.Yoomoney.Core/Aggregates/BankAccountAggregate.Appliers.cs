using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Projections;

namespace ES.Yoomoney.Core.Aggregates;

public sealed partial class BankAccountAggregate
{
    private BankAccountAggregate Apply(DomainEvents.InvoiceCreatedDomainEvent @event)
    {
        IncreaseVersion();

        TotalEvents.Add(@event);

        return this;
    }
    
    private BankAccountAggregate Apply(DomainEvents.AccountBalanceInitializedTo @event)
    {
        Id = @event.StreamId;
        
        IncreaseVersion();

        TotalEvents.Add(@event);

        return this;
    }

    private BankAccountAggregate Apply(DomainEvents.MoneyDepositedDomainEvent @event)
    {
        Balance += @event.Amount;

        IncreaseVersion();

        TotalEvents.Add(@event);

        return this;
    }

    private BankAccountAggregate Apply(DomainEvents.MoneyWithdrawnDomainEvent @event)
    {
        Balance -= @event.Amount;

        IncreaseVersion();

        TotalEvents.Add(@event);

        return this;
    }
}