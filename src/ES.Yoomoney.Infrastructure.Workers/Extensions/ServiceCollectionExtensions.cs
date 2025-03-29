using ES.Yoomoney.Core.Extensions;
using ES.Yoomoney.Infrastructure.Workers.Options;
using ES.Yoomoney.Infrastructure.Workers.Workers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Workers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerInfrastructureWorkers(this IServiceCollection services)
    {
        services.AddHostedService<PaymentsPaidProcessingWorker>();
        services.AddApplicationOptions<BackgroundWorkerOptions>(BackgroundWorkerOptions.Section);

        return services;
    }
}