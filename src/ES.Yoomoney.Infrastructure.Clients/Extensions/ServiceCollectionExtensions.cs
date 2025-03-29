using ES.Yoomoney.Core.Abstractions;

using Microsoft.Extensions.DependencyInjection;

using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Clients.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerInfrastructureClients(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, YoomoneyInvoiceService>();
        services.AddSingleton(
            new Client(
                shopId: "999274",
                secretKey: "test_PHF9bx_r4kRt7Zpf1U9ZaJ5CAhGqxcy0JwWjReNsBhs"));

        return services;
    }
}