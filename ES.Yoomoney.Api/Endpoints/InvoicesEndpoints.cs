using ES.Yoomoney.Api.Abstractions;
using ES.Yoomoney.Application.Features.Commands;
using ES.Yoomoney.Application.Features.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ES.Yoomoney.Api.Endpoints;

public sealed class InvoicesEndpoints: IHasEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/invoices", CreateInvoice);
        builder.MapGet("/invoices", GetInvoices);
    }

    private async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand.Request request, 
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request, ct);

        return TypedResults.Ok(result);
    }
    
    private async Task<IResult> GetInvoices(
        [FromServices] ISender sender,
        [FromQuery] Guid accountId,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var request = new GetInvoicesQuery.Request(accountId, page, pageSize);
        var result = await sender.Send(request, ct);

        return TypedResults.Ok(result);
    }
}