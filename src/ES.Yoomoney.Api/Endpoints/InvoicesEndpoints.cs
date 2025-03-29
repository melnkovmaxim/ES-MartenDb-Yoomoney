using ES.Yoomoney.Application.Features.Commands;
using ES.Yoomoney.Application.Features.Queries;
using ES.Yoomoney.Common.Presentation.MinimapApi.Abstractions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace ES.Yoomoney.Api.Endpoints;

public sealed class InvoicesEndpoints : IHasEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/v1/invoices", CreateInvoice);
        builder.MapGet("/api/v1/invoices", GetInvoices);
    }

    private static async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand.Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetInvoices(
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