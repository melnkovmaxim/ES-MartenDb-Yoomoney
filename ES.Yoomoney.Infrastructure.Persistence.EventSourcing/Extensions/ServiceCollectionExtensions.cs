using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.DomainEvents;
using ES.Yoomoney.Core.Projections;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Infrastructure.Persistence.EventSourcing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLayerInfrastructurePersistenceEventSourcing(this IServiceCollection services)
    {
        services.AddMarten(options =>
            {
                options.Events.AddEventType<DebitBalanceDomainEvent>();
                options.Events.AddEventType<TopupBalanceDomainEvent>();
                options.Events.AddEventType<AccountBalanceInitializedEvent>();
                options.Schema.For<BalanceProjection>().Identity(x => x.AccountId);
            })
            .UseNpgsqlDataSource();
        services.AddScoped<IEsUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        return services;
    }
}