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
        var @event = new DomainEvents.AccountBalanceInitializedTo(bankAccountId);

        bankAccount.Apply(@event);
        bankAccount.UncommittedEvents.Add(@event);

        return bankAccount;
    }

    public void CreateInvoice(Guid invoiceId, decimal amount)
    {
        var @event = new DomainEvents.InvoiceCreatedDomainEvent(invoiceId, Id, amount);
        
        Apply(@event);
        
        UncommittedEvents.Add(@event);
    }

    public void Deposit(
        Guid invoiceId, 
        decimal amount, 
        string currency,
        Payment meta)
    {
        var @event = new DomainEvents.MoneyDepositedDomainEvent(
            invoiceId, 
            Id, 
            amount, 
            currency, 
            meta);
        
        Apply(@event);
        
        UncommittedEvents.Add(@event);
    }

    public void Withdrawn(decimal amount)
    {
        var operationId = Guid.CreateVersion7();
        var @event = new DomainEvents.MoneyWithdrawnDomainEvent(operationId, Id, amount);
        
        Apply(@event);
        
        UncommittedEvents.Add(@event);
    }
}