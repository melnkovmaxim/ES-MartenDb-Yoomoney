using ES.Yoomoney.Application.Features.Commands;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using ES.Yoomoney.Core.Entities;
using ES.Yoomoney.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace ES.Yoomoney.Tests.Integration.Features.Commands;

public class CreateInvoiceCommandIntegrationTests : IClassFixture<AppWebFactory>
{
    private readonly IEsEventStore _eventStore;
    private readonly IPaymentService _paymentService;

    public CreateInvoiceCommandIntegrationTests(AppWebFactory factory)
    {
        var scope = factory.Services.CreateScope();
        
        _eventStore = scope.ServiceProvider.GetRequiredService<IEsEventStore>();
        _paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
    }
} 