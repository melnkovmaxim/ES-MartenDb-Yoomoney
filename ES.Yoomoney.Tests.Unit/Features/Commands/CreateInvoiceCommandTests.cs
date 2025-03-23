using ES.Yoomoney.Application.Features.Commands;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using Moq;
using Xunit;
using Shouldly;

namespace ES.Yoomoney.Tests.Unit.Features.Commands;

public class CreateInvoiceCommandTests
{
    private readonly Mock<IEsEventStore> _eventStoreMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly CreateInvoiceCommand.Handler _handler;

    public CreateInvoiceCommandTests()
    {
        _eventStoreMock = new Mock<IEsEventStore>();
        _paymentServiceMock = new Mock<IPaymentService>();
        
        _handler = new CreateInvoiceCommand.Handler(
            _eventStoreMock.Object,
            _paymentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldCreateInvoiceAndUpdateBankAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = 1000m;
        var paymentId = "test-payment-id";
        var confirmationUrl = "https://test.com/confirm";

        var request = new CreateInvoiceCommand.Request(accountId, amount);
        var payment = (PaymentId: paymentId, ConfirmationUrl: confirmationUrl);
        var bankAccount = BankAccountAggregate.Open(accountId);

        _paymentServiceMock
            .Setup(x => x.CreateInvoiceAsync(amount))
            .ReturnsAsync(payment);

        _eventStoreMock
            .Setup(x => x.LoadAsync<BankAccountAggregate>(accountId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bankAccount);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.PaymentId.ShouldBe(paymentId);
        result.ConfirmationUrl.ShouldBe(confirmationUrl);
        
        _paymentServiceMock.Verify(x => x.CreateInvoiceAsync(amount), Times.Once);
        _eventStoreMock.Verify(x => x.LoadAsync<BankAccountAggregate>(accountId, null, It.IsAny<CancellationToken>()), Times.Once);
        _eventStoreMock.Verify(x => x.StoreAsync(It.Is<BankAccountAggregate>(a => a.Balance == amount), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBankAccountDoesNotExist_ShouldCreateNewBankAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var amount = 1000m;
        var paymentId = "test-payment-id";
        var confirmationUrl = "https://test.com/confirm";

        var request = new CreateInvoiceCommand.Request(accountId, amount);
        var payment = (PaymentId: paymentId, ConfirmationUrl: confirmationUrl);

        _paymentServiceMock
            .Setup(x => x.CreateInvoiceAsync(amount))
            .ReturnsAsync(payment);

        _eventStoreMock
            .Setup(x => x.LoadAsync<BankAccountAggregate>(accountId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BankAccountAggregate)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.PaymentId.ShouldBe(paymentId);
        result.ConfirmationUrl.ShouldBe(confirmationUrl);
        
        _eventStoreMock.Verify(x => x.StoreAsync(It.Is<BankAccountAggregate>(a => a.Balance == amount), It.IsAny<CancellationToken>()), Times.Once);
    }
} 