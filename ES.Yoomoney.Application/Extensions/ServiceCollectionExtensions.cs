using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerApplication(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssembly(CurrentAssembly.Reference));
        
        return services;
    }
}