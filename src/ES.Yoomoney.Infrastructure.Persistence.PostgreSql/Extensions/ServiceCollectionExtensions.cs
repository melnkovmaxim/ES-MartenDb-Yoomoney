using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Persistence.PostgreSql.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistencePostgresql(this IServiceCollection services)
    {
        _ = services.AddSingleton(new PostgresqlConnectionFactory("postgresql"));

        return services;
    }
}