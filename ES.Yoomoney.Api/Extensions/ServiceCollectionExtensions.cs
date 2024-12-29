using System.Reflection;
using ES.Yoomoney.Api.Abstractions;
using JasperFx.Core.IoC;

namespace ES.Yoomoney.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        services
            .Scan(scan => scan
                .FromAssemblyDependencies(assembly)
                .AddClasses(classes => classes.AssignableTo<IHasEndpoints>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
    }
}