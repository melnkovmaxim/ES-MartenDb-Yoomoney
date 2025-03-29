using ES.Yoomoney.Application.Features.Queries;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;

using Moq;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Tests.Unit.Features.Queries;

public class GetInvoicesQueryTests
{
    private readonly Mock<IEsEventStore> _eventStoreMock;
    private readonly GetInvoicesQuery.Handler _handler;

    public GetInvoicesQueryTests()
    {
        _eventStoreMock = new Mock<IEsEventStore>();
        _handler = new GetInvoicesQuery.Handler(_eventStoreMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountExists_ReturnsPayments()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var bankAccount = BankAccountAggregate.Open(accountId);

        // Add some test events
        var firstInvoiceId = Guid.CreateVersion7();
        var secondInvoiceId = Guid.CreateVersion7();
        var thirdInvoiceId = Guid.CreateVersion7();

        bankAccount.Deposit(firstInvoiceId, amount: 1000, "RUR", new Payment());
        bankAccount.Deposit(secondInvoiceId, amount: 2000, "EUR", new Payment());
        bankAccount.Deposit(thirdInvoiceId, amount: 3000, "UAH", new Payment());

        _eventStoreMock
            .Setup(x => x.LoadAsync<BankAccountAggregate>(accountId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bankAccount);

        var request = new GetInvoicesQuery.Request(accountId, Page: 1, PageSize: 3);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.TotalCount.ShouldBe(3);
        response.Invoices.Count.ShouldBe(3);

        var invoices = response.Invoices.ToList();

        invoices[0].InvoiceId.ShouldBe(firstInvoiceId);
        invoices[1].InvoiceId.ShouldBe(secondInvoiceId);
        invoices[2].InvoiceId.ShouldBe(thirdInvoiceId);

        invoices[0].Amount.ShouldBe(1000);
        invoices[1].Amount.ShouldBe(2000);
        invoices[2].Amount.ShouldBe(3000);

        invoices[0].Currency.ShouldBe("RUR");
        invoices[1].Currency.ShouldBe("EUR");
        invoices[2].Currency.ShouldBe("UAH");
    }

    [Fact]
    public async Task Handle_WhenAccountDoesNotExist_ReturnsEmptyResult()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = new GetInvoicesQuery.Request(accountId);

        _eventStoreMock
            .Setup(x => x.LoadAsync<BankAccountAggregate>(accountId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BankAccountAggregate)null!);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.Invoices.ShouldBeEmpty();
        response.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var bankAccount = BankAccountAggregate.Open(accountId);

        // Add 5 test events
        for (var i = 1; i <= 5; i++)
        {
            bankAccount.Deposit(Guid.CreateVersion7(), i * 1000, "RUR", new Payment());
        }

        _eventStoreMock
            .Setup(x => x.LoadAsync<BankAccountAggregate>(accountId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bankAccount);

        var request = new GetInvoicesQuery.Request(accountId, Page: 2, PageSize: 2);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.TotalCount.ShouldBe(5);
        response.Invoices.Count.ShouldBe(2);

        var invoices = response.Invoices.ToList();
        invoices[0].Amount.ShouldBe(3000);
        invoices[1].Amount.ShouldBe(4000);
    }
}