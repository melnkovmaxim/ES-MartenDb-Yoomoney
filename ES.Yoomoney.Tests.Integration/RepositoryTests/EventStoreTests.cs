using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using FluentAssertions;
using Marten;

namespace ES.Yoomoney.Tests.Integration.RepositoryTests;

[Collection(nameof(AppWebFactory))]
public sealed class EventStoreTests(AppWebFactory factory)
{
    [Fact]
    public async Task Test()
    {
        // var unitOfWork = factory.Resolve<IEsUnitOfWork>();
        // var session = factory.Resolve<IDocumentSession>();
        // var eventStore = unitOfWork.CreateEventStore();
        // var streamId = Guid.NewGuid();
        //
        // await eventStore.AddEventAsync(new AccountBalanceInitializedEvent(streamId));
        // await eventStore.AddEventAsync(new DebitBalanceDomainEvent(streamId, 100, PaymentSystemsEnum.Yoomoney, null));
        // await unitOfWork.CommitAsync();
        // var projection = await eventStore.GetProjectionAndCreateSnapshotAsync<BalanceProjection>(streamId, CancellationToken.None);
        // var events = await session.Events.FetchStreamAsync(streamId);

        var eventStore = factory.Resolve<IEsEventStore>();
        var bankAccount = BankAccountAggregate.Open();
        
        bankAccount.Deposit(50);
        bankAccount.Withdrawn(10);

        await eventStore.StoreAsync(bankAccount, CancellationToken.None);
        var persistedBankAccount =
            await eventStore.LoadAsync<BankAccountAggregate>(bankAccount.Id, null, CancellationToken.None);

        persistedBankAccount.Balance.Should().Be(40);
        
        persistedBankAccount.Deposit(1000);
        persistedBankAccount.Deposit(50); 
        persistedBankAccount.Withdrawn(500);
        persistedBankAccount.Deposit(1);
        
        await eventStore.StoreAsync(persistedBankAccount, CancellationToken.None);
        
        var persistedBankAccount2 =
            await eventStore.LoadAsync<BankAccountAggregate>(bankAccount.Id, null, CancellationToken.None);

        persistedBankAccount2.Balance.Should().Be(591);
    }
}