using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Infrastructure.Persistence.EventSourcing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerInfrastructurePersistenceEventSourcing(this IServiceCollection services)
    {
        services.AddMarten(options =>
            {
                options.Events.AddEventType<Events.MoneyDepositedDomainEvent>();
                options.Events.AddEventType<Events.MoneyWithdrawnDomainEvent>();
                options.Events.AddEventType<Events.AccountBalanceInitializedTo>();
                // options.Schema.For<BalanceProjection>().Identity(x => x.AccountId);
            })
            .UseNpgsqlDataSource();
        services.AddScoped<IEsUnitOfWork, UnitOfWork>();
        services.AddSingleton<IEsEventStore, EventStore>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        return services;
    }
}