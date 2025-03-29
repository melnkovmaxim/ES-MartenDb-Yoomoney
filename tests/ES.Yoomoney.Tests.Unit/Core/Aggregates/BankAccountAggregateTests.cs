using ES.Yoomoney.Core.Aggregates;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Tests.Unit.Core.Aggregates;

public class BankAccountAggregateTests
{
    [Fact]
    public void Open_ShouldCreateNewAccountWithInitialBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        var account = BankAccountAggregate.Open(accountId);
        var uncommittedEvents = account.GetUncommittedEvents();

        // Assert
        Assert.Equal(accountId, account.Id);
        Assert.Equal(expected: 0, account.Balance);
        Assert.Single(uncommittedEvents);
        Assert.IsType<DomainEvents.AccountBalanceInitializedTo>(uncommittedEvents.First());
    }

    [Fact]
    public void CreateInvoice_ShouldCreateInvoiceEvent()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var amount = 100.00m;
        var account = BankAccountAggregate.Open(accountId);
        account.GetUncommittedEvents(); // Clear initial event

        // Act
        account.CreateInvoice(invoiceId, amount);
        var uncommittedEvents = account.GetUncommittedEvents();

        // Assert
        Assert.Single(uncommittedEvents);
        var invoiceEvent = uncommittedEvents.First() as DomainEvents.InvoiceCreatedDomainEvent;
        Assert.NotNull(invoiceEvent);
        Assert.Equal(invoiceId, invoiceEvent.InvoiceId);
        Assert.Equal(accountId, invoiceEvent.AccountId);
        Assert.Equal(amount, invoiceEvent.Amount);
    }

    [Fact]
    public void Deposit_ShouldCreateDepositEvent()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var amount = 100.00m;
        var currency = "RUB";
        var payment = new Payment
        {
            Id = "PaymentId",
            Amount = new Amount { Value = amount, Currency = currency }
        };
        var account = BankAccountAggregate.Open(accountId);
        account.GetUncommittedEvents(); // Clear initial event

        // Act
        account.Deposit(invoiceId, amount, currency, payment);
        var uncommittedEvents = account.GetUncommittedEvents();

        // Assert
        Assert.Single(uncommittedEvents);
        var depositEvent = uncommittedEvents.First() as DomainEvents.MoneyDepositedDomainEvent;
        Assert.NotNull(depositEvent);
        Assert.Equal(invoiceId, depositEvent.InvoiceId);
        Assert.Equal(accountId, depositEvent.AccountId);
        Assert.Equal(amount, depositEvent.Amount);
        Assert.Equal(currency, depositEvent.Currency);
        Assert.Equal(payment, depositEvent.Meta);
    }

    [Fact]
    public void Withdrawn_ShouldCreateWithdrawalEvent()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = 50.00m;
        var account = BankAccountAggregate.Open(accountId);
        account.GetUncommittedEvents(); // Clear initial event

        // Act
        account.Withdrawn(amount);
        var uncommittedEvents = account.GetUncommittedEvents();

        // Assert
        Assert.Single(uncommittedEvents);
        var withdrawalEvent = uncommittedEvents.First() as DomainEvents.MoneyWithdrawnDomainEvent;
        Assert.NotNull(withdrawalEvent);
        Assert.Equal(accountId, withdrawalEvent.AccountId);
        Assert.Equal(amount, withdrawalEvent.Amount);
    }
}