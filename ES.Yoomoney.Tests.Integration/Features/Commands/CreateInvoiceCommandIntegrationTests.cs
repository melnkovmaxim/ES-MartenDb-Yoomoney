using ES.Yoomoney.Application.Features.Commands;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Core.Entities;
using ES.Yoomoney.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace ES.Yoomoney.Tests.Integration.Features.Commands;

public class CreateInvoiceCommandIntegrationTests : IClassFixture<AppWebFactory>
{
    private readonly IEsEventStore _eventStore;
    private readonly IPaymentService _paymentService;

    public CreateInvoiceCommandIntegrationTests(AppWebFactory factory)
    {
        var scope = factory.Services.CreateScope();
        
        _eventStore = scope.ServiceProvider.GetRequiredService<IEsEventStore>();
        _paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateInvoiceAndUpdateBankAccount()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var amount = 1000m;
        var request = new CreateInvoiceCommand.Request(accountId, amount);
        var handler = new CreateInvoiceCommand.Handler(_eventStore, _paymentService);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.PaymentId.ShouldNotBeNullOrEmpty();
        result.ConfirmationUrl.ShouldNotBeNullOrEmpty();

        // Verify bank account state
        var bankAccount = await _eventStore.LoadAsync<BankAccountAggregate>(accountId, null, CancellationToken.None);
        bankAccount.ShouldNotBeNull();
        bankAccount.Balance.ShouldBe(amount);
    }

    [Fact]
    public async Task Handle_WhenMultipleRequests_ShouldAccumulateBalance()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var amount1 = 1000m;
        var amount2 = 2000m;
        var handler = new CreateInvoiceCommand.Handler(_eventStore, _paymentService);

        // Act
        var result1 = await handler.Handle(new CreateInvoiceCommand.Request(accountId, amount1), CancellationToken.None);
        var result2 = await handler.Handle(new CreateInvoiceCommand.Request(accountId, amount2), CancellationToken.None);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.PaymentId.ShouldNotBeNullOrEmpty();
        result2.PaymentId.ShouldNotBeNullOrEmpty();
        result1.PaymentId.ShouldNotBe(result2.PaymentId);

        var bankAccount = await _eventStore.LoadAsync<BankAccountAggregate>(accountId, null, CancellationToken.None);
        bankAccount.ShouldNotBeNull();
        bankAccount.Balance.ShouldBe(amount1 + amount2);
    }
} 