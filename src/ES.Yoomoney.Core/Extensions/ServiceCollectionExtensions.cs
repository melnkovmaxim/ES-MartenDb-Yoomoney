using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationOptions<TOptions>(
        this IServiceCollection services,
        string section)
        where TOptions : class, new()
    {
        _ = services
            .AddOptions<TOptions>()
            .BindConfiguration(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}