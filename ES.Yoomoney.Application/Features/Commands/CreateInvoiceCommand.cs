using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.DomainEvents;
using ES.Yoomoney.Core.Entities;
using MediatR;

namespace ES.Yoomoney.Application.Features.Commands;

public static class CreateInvoiceCommand
{
    public sealed record Request(Guid AccountId, decimal Amount) : IRequest<Response>;
    public sealed record Response(string PaymentId, string ConfirmationUrl);

    public sealed class Handler(
        IEsUnitOfWork unitOfWork,
        IRepository<AccountPaymentEntity> accountPaymentRepository,
        IPaymentService paymentService) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var payment = await paymentService.CreateInvoiceAsync(request.Amount);
            var @event = new AccountBalanceInitializedEvent(request.AccountId);
            var eventStore = unitOfWork.CreateEventStore();

            var accountPayment = await accountPaymentRepository.GetFirstOrDefaultAsync(request.AccountId.ToString(), cancellationToken);

            if (accountPayment is null)
            {
                var newAccountPayment = new AccountPaymentEntity(payment.PaymentId, request.AccountId);
                
                await accountPaymentRepository.UpsertAsync(newAccountPayment, cancellationToken);
            }
            
            await eventStore.AddEventAsync(@event);
            await unitOfWork.CommitAsync();
            
            return new Response(payment.PaymentId, payment.ConfirmationUrl);
        }
    }
}