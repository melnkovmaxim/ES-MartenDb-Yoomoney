using System.Text.Json;
using Confluent.Kafka;
using ES.Yoomoney.Application.Features.Commands;
using ES.Yoomoney.Application.Features.Events;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Core.IntegrationEvents;
using KafkaFlow;
using MediatR;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Messaging.Consumers;

public sealed class InvoiceStatusChangedConsumer(IPublisher mediator)
    : IMessageHandler<InvoiceStatusChangedIntegrationEvent>
{
    public async Task Handle(IMessageContext context, InvoiceStatusChangedIntegrationEvent message)
    {
        await mediator.Publish(message);
    }
}