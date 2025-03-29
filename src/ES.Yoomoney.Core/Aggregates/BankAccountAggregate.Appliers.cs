namespace ES.Yoomoney.Core.Aggregates;

#pragma warning disable S3241
public sealed partial class BankAccountAggregate
{
    private BankAccountAggregate Apply(DomainEvents.InvoiceCreatedDomainEvent @event)
    {
        IncreaseVersion();

        _ = TotalEvents.Add(@event);

        return this;
    }

    private BankAccountAggregate Apply(DomainEvents.AccountBalanceInitializedTo @event)
    {
        Id = @event.StreamId;

        IncreaseVersion();

        _ = TotalEvents.Add(@event);

        return this;
    }

    private BankAccountAggregate Apply(DomainEvents.MoneyDepositedDomainEvent @event)
    {
        Balance += @event.Amount;

        IncreaseVersion();

        _ = TotalEvents.Add(@event);

        return this;
    }

    private BankAccountAggregate Apply(DomainEvents.MoneyWithdrawnDomainEvent @event)
    {
        Balance -= @event.Amount;

        IncreaseVersion();

        _ = TotalEvents.Add(@event);

        return this;
    }
}
#pragma warning restore S3241