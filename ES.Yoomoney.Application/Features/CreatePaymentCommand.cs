using ES.Yoomoney.Core.Abstractions;
using MediatR;

namespace ES.Yoomoney.Application.Features
{
    public sealed record CreatePaymentCommand(Guid AccountId, decimal Amount): IRequest;

    public class CreatePaymentCommandHandler(IPaymentService paymentService) : IRequestHandler<CreatePaymentCommand>
    {
        public async Task Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentId = await paymentService.CreatePaymentAsync(request.Amount);
            
        }
    }
}
