using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Core.Entities;
using MediatR;

namespace ES.Yoomoney.Application.Features.Commands;

public static class CreateInvoiceCommand
{
    public sealed record Request(Guid AccountId, decimal Amount) : IRequest<Response>;

    public sealed record Response(string PaymentId, string ConfirmationUrl);

    public sealed class Handler(
        IEsEventStore eventStore,
        IPaymentService paymentService) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var payment = await paymentService.CreateInvoiceAsync(request.Amount);
            var bankAccount = await eventStore.LoadAsync<BankAccountAggregate>(request.AccountId, version: null, cancellationToken) 
                              ?? BankAccountAggregate.Open(request.AccountId);

            bankAccount.Deposit(request.Amount);
            
            await eventStore.StoreAsync(bankAccount, cancellationToken);

            return new Response(payment.PaymentId, payment.ConfirmationUrl);
        }
    }
}