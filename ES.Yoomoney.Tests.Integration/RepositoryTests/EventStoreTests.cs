using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.DomainEvents;
using ES.Yoomoney.Core.Enums;
using ES.Yoomoney.Core.Projections;
using FluentAssertions;
using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Tests.Integration.RepositoryTests;

[Collection(nameof(AppWebFactory))]
public sealed class EventStoreTests(AppWebFactory factory)
{
    [Fact]
    public async Task Test()
    {
        var unitOfWork = factory.Resolve<IEsUnitOfWork>();
        var session = factory.Resolve<IDocumentSession>();
        var eventStore = unitOfWork.CreateEventStore();
        var streamId = Guid.NewGuid();

        await eventStore.AddEventAsync(new AccountBalanceInitializedEvent(streamId));
        await eventStore.AddEventAsync(new DebitBalanceDomainEvent(streamId, 100, PaymentSystemsEnum.Yoomoney, null));
        await unitOfWork.CommitAsync();
        var projection = await eventStore.GetProjectionAndCreateSnapshotAsync<BalanceProjection>(streamId, CancellationToken.None);
        var events = await session.Events.FetchStreamAsync(streamId);
        

        projection.IsSuccess.Should().Be(true);
        projection.Value.Amount.Should().Be(100);
    }
}