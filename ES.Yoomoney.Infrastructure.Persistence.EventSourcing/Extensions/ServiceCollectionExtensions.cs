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
                // TODO: разобраться почему не работает Key / Unique Index
                options.Schema.For<DomainEvents.Event>()
                    .Identity(e => e.EventId)
                    .UniqueIndex(e => e.EventId);
                options.Events.EnableUniqueIndexOnEventId = true;
                options.Events.AddEventType<DomainEvents.MoneyDepositedDomainEvent>();
                options.Events.AddEventType<DomainEvents.MoneyWithdrawnDomainEvent>();
                options.Events.AddEventType<DomainEvents.AccountBalanceInitializedTo>();
                // options.Schema.For<BalanceProjection>().Identity(x => x.AccountId);
            })
            .UseNpgsqlDataSource();
        services.AddScoped<IEsUnitOfWork, UnitOfWork>();
        services.AddSingleton<IEsEventStore, EventStore>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        return services;
    }
}