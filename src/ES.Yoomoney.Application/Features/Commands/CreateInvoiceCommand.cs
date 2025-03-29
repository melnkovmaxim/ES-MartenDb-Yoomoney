using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;

using MediatR;

using Microsoft.Extensions.Logging;

namespace ES.Yoomoney.Application.Features.Commands;

public static class CreateInvoiceCommand
{
    public sealed record Request(Guid AccountId, decimal Amount) : IRequest<Response>;

    public sealed record Response(Guid PaymentId, Uri ConfirmationUrl);

    public sealed class Handler(
        IEsEventStore eventStore,
        IInvoiceService invoiceService,
        ILogger<Handler> logger) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            using var _ = logger.BeginAccountScope(request.AccountId);

            var (paymentId, confirmationUrl) = await invoiceService
                .CreateInvoiceAsync(request.AccountId, request.Amount);
            var bankAccount = await eventStore.LoadAsync<BankAccountAggregate>(
                    request.AccountId,
                    version: null,
                    cancellationToken)
                ?? BankAccountAggregate.Open(request.AccountId);

            if (bankAccount
                .GetUncommittedEvents()
                .OfType<DomainEvents.AccountBalanceInitializedTo>()
                .Any())
            {
                logger.AccountCreated();
            }

            bankAccount.CreateInvoice(paymentId, request.Amount);

            await eventStore.StoreAsync(bankAccount, cancellationToken);

            logger.InvoiceCreated(paymentId, request.Amount, confirmationUrl);

            return new Response(paymentId, confirmationUrl);
        }
    }
}