using System.Reflection;

using ES.Yoomoney.Common.Presentation.MinimapApi.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Common.Presentation.MinimapApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        _ = services
            .Scan(
                scan => scan
                    .FromAssemblyDependencies(assembly)
                    .AddClasses(classes => classes.AssignableTo<IHasEndpoints>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
    }
}