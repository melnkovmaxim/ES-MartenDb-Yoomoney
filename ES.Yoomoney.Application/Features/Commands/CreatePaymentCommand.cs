using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.DomainEvents;
using MediatR;

namespace ES.Yoomoney.Application.Features.Commands;

public static class CreatePaymentCommand
{
    public sealed record Request(Guid AccountId, decimal Amount) : IRequest<Response>;
    public sealed record Response(string PaymentId, string ConfirmationUrl);

    public sealed class Handler(
        IEsUnitOfWork unitOfWork,
        IPaymentService paymentService) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var payment = await paymentService.CreatePaymentAsync(request.Amount);
            var @event = new AccountBalanceInitializedEvent(request.AccountId);
            var eventStore = unitOfWork.CreateEventStore();

            await eventStore.AddEventAsync(@event);
            await unitOfWork.CommitAsync();
            
            return new Response(payment.PaymentId, payment.ConfirmationUrl);
        }
    }
}