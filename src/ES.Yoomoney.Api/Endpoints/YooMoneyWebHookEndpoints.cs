using System.Net.Mime;
using System.Text.Json;

using ES.Yoomoney.Common.Presentation.MinimapApi.Abstractions;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.IntegrationEvents;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Api.Endpoints;

public sealed class YooMoneyWebHookEndpoints : IHasEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/v1/yoomoney/receive-payment", ReceivePayment);
    }

    private static async Task<NoContent> ReceivePayment(
        [FromForm] object payment,
        [FromServices] IEventBus<InvoiceStatusChangedIntegrationEvent> eventBus,
        CancellationToken ct)
    {
        var notification = Client.ParseMessage(
            HttpMethod.Post.Method,
            MediaTypeNames.Application.FormUrlEncoded,
            JsonSerializer.Serialize(payment));

        if (notification is PaymentSucceededNotification succeededPayment)
        {
            var paymentPaidEvent = new InvoiceStatusChangedIntegrationEvent(succeededPayment.Object);

            await eventBus.ProduceAsync(paymentPaidEvent, ct);

            return TypedResults.NoContent();
        }

        // TODO: log
        return TypedResults.NoContent();
    }
}