using ES.Yoomoney.Api.Abstractions;
using ES.Yoomoney.Application.Features.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ES.Yoomoney.Api.Endpoints;

public sealed class PaymentEndpoints: IHasEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/payments", CreatePayment);
    }

    private async Task<IResult> CreatePayment(
        [FromBody] CreatePaymentCommand.Request request, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request, ct);

        return TypedResults.Ok(result);
    }
}