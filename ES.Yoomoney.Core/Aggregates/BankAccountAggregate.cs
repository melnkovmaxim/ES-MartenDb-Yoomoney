using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.Aggregates;

public sealed partial class BankAccountAggregate: Aggregate
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }

    public BankAccountAggregate()
    {
        Id = Guid.NewGuid();

        Apply(new Events.AccountBalanceInitializedTo(Id));
    }
}