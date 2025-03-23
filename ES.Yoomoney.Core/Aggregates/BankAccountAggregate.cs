using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Enums;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Core.Aggregates;

public sealed partial class BankAccountAggregate: Aggregate
{
    public decimal Balance { get; private set; }

    private BankAccountAggregate()
    {
    }

    public static BankAccountAggregate Open(Guid accountId)
    {
        var bankAccount = new BankAccountAggregate();
        var bankAccountId = accountId;
        var @event = new Events.AccountBalanceInitializedTo(bankAccountId);

        bankAccount.Apply(@event);
        bankAccount.UncommittedEvents.Add(@event);

        return bankAccount;
    }

    public void Deposit(decimal amount)
    {
        var @event = new Events.MoneyDepositedDomainEvent(Id, amount, PaymentSystemsEnum.Yoomoney, new Payment());
        
        Apply(@event);
        
        UncommittedEvents.Add(@event);
    }

    public void Withdrawn(decimal amount)
    {
        var @event = new Events.MoneyWithdrawnDomainEvent(Id, amount);
        
        Apply(new Events.MoneyWithdrawnDomainEvent(Id, amount));
        
        UncommittedEvents.Add(@event);
    }
}