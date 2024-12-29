using ES.Yoomoney.Infrastructure.Workers.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Workers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerInfrastructureWorkers(this IServiceCollection services)
    {
        services.AddHostedService<PaymentsPaidProcessingWorker>();

        return services;
    }
}