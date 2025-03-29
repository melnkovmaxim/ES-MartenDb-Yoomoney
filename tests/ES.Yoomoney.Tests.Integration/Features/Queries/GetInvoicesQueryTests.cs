using Bogus;

using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Application.Features.Queries;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Tests.Integration.Features.Queries;

public class GetInvoicesQueryTests(AppWebFactory factory) : IClassFixture<AppWebFactory>
{
    private readonly IEsEventStore _eventStore = factory.Services.GetRequiredService<IEsEventStore>();
    private readonly IMediator _mediator = factory.Services.GetRequiredService<IMediator>();

    [Fact]
    public async Task Handle_WhenAccountExists_ReturnsPayments()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);

        // Add some test events
        var firstInvoiceId = Guid.CreateVersion7();
        var secondInvoiceId = Guid.CreateVersion7();
        var thirdInvoiceId = Guid.CreateVersion7();

        var firstCurrency = new Faker().Random.String2(3);
        var secondCurrency = new Faker().Random.String2(3);
        var thirdCurrency = new Faker().Random.String2(3);
        var meta = new Faker<Payment>().Generate();

        bankAccount.Deposit(firstInvoiceId, 1000, firstCurrency, meta);
        bankAccount.Deposit(secondInvoiceId, 2000, secondCurrency, meta);
        bankAccount.Deposit(thirdInvoiceId, 3000, thirdCurrency, meta);

        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Act
        var request = new GetInvoicesQuery.Request(accountId, Page: 1, PageSize: 3);
        var response = await _mediator.Send(request, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.TotalCount.ShouldBe(3);
        response.TotalCount.ShouldBe(response.Invoices.Count);

        var invoices = response.Invoices.ToList();

        invoices[0].InvoiceId.ShouldBe(firstInvoiceId);
        invoices[1].InvoiceId.ShouldBe(secondInvoiceId);
        invoices[2].InvoiceId.ShouldBe(thirdInvoiceId);

        invoices[0].Amount.ShouldBe(1000);
        invoices[1].Amount.ShouldBe(2000);
        invoices[2].Amount.ShouldBe(3000);

        invoices[0].Currency.ShouldBe(firstCurrency);
        invoices[1].Currency.ShouldBe(secondCurrency);
        invoices[2].Currency.ShouldBe(thirdCurrency);
    }

    [Fact]
    public async Task Handle_WhenAccountDoesNotExist_ReturnsEmptyResult()
    {
        // Arrange
        var nonExistentAccountId = Guid.CreateVersion7();
        var request = new GetInvoicesQuery.Request(nonExistentAccountId);

        // Act
        var response = await _mediator.Send(request, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.Invoices.ShouldBeEmpty();
        response.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);

        // Add 5 test events
        for (int i = 1; i <= 5; i++)
        {
            var currency = new Faker().Random.String2(3);
            var meta = new Faker<Payment>().Generate();

            bankAccount.Deposit(Guid.CreateVersion7(), i * 1000, currency, meta);
        }

        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Act - Get second page with 2 items per page
        var request = new GetInvoicesQuery.Request(accountId, Page: 2, PageSize: 2);
        var response = await _mediator.Send(request, CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.TotalCount.ShouldBe(5);
        response.Invoices.Count.ShouldBe(2);

        var invoices = response.Invoices.ToList();
        invoices[0].Amount.ShouldBe(3000);
        invoices[1].Amount.ShouldBe(4000);
    }
}