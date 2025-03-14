using ES.Yoomoney.Core.Abstractions;

namespace ES.Yoomoney.Core.Aggregates;

public sealed partial class BankAccountAggregate: IAggregate
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }

    private List<Events.Event> Events { get; } = [];

    public BankAccountAggregate()
    {
        Id = Guid.NewGuid();
        
        Events.Add(new Events.AccountBalanceInitializedTo(Id));
    }
}