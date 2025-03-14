using ES.Yoomoney.Api.Abstractions;
using ES.Yoomoney.Application.Features.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ES.Yoomoney.Api.Endpoints;

public sealed class PaymentEndpoints: IHasEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/invoices", CreateInvoice);
    }

    private async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand.Request request, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request, ct);

        return TypedResults.Ok(result);
    }
}