using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Entities;
using ES.Yoomoney.Core.IntegrationEvents;
using MediatR;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Application.Features.IntegrationEvents;

internal sealed class PaymentReceivedIntegrationEventHandler(
    IPaymentService paymentService,
    IEsUnitOfWork unitOfWork,
    IRepository<AccountPaymentEntity> accountPaymentRepository,
    IRepository<UnprocessedPaymentEntity> unprocesedPaymentRepository): INotificationHandler<PaymentReceivedIntegrationEvent>
{
    public async Task Handle(PaymentReceivedIntegrationEvent @event, CancellationToken ct)
    {
        var payments = @event.paidPayments;

        // TODO: сделать параллельно так как там много асинхронщины
        foreach (var payment in payments)
        {
            await ProcessPaymentAsync(payment, ct);
        }
    }

    private async Task ProcessPaymentAsync(Payment payment, CancellationToken ct)
    {
        var accountPaymentEntity = await accountPaymentRepository.GetFirstOrDefaultAsync(payment.Id, ct);

        // if (accountPaymentEntity is null)
        // {
        //     var unprocessedPayment = new UnprocessedPaymentEntity(payment.Id, payment);
        //     
        //     await unprocesedPaymentRepository.UpsertAsync(unprocessedPayment, ct);
        //     await paymentService.CapturePaymentsAsync(payment.Id);
        //     
        //     return;
        // }
        //
        // var debitBalanceEvent = new DebitBalanceDomainEvent(accountPaymentEntity.AccountId, payment.Amount.Value, PaymentSystemsEnum.Yoomoney, payment);
        //
        // var eventStore = unitOfWork.CreateEventStore();
        //
        // await eventStore.AddEventAsync(debitBalanceEvent);
        // await unitOfWork.CommitAsync();
        // await paymentService.CapturePaymentsAsync(payment.Id);
        //
        // accountPaymentEntity.SetIsProcessed();
        //
        // await accountPaymentRepository.UpsertAsync(accountPaymentEntity, CancellationToken.None);
    }
}