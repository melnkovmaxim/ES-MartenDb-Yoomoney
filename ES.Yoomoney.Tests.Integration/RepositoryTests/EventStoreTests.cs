using Bogus;
using ES.Yoomoney.Core.Abstractions;
using ES.Yoomoney.Core.Aggregates;
using FluentAssertions;
using Marten;
using Marten.Services;
using Marten.Events;
using Marten.Exceptions;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Tests.Integration.RepositoryTests;

[Collection(nameof(AppWebFactory))]
public sealed class EventStoreTests(AppWebFactory factory)
{
    private readonly Faker _faker = new();
    private readonly IEsEventStore _eventStore = factory.Resolve<IEsEventStore>();
    private readonly IDocumentSession _session = factory.Resolve<IDocumentSession>();

    [Fact]
    public async Task StoreAndLoad_WhenValidAggregate_ShouldPersistAndRestoreState()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        var invoiceId = _faker.Random.Guid();
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        bankAccount.Deposit(invoiceId, 50, currency, meta);
        bankAccount.Withdrawn(10);

        // Act
        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);
        var persistedBankAccount =
            await _eventStore.LoadAsync<BankAccountAggregate>(bankAccount.Id, null, CancellationToken.None);

        // Assert
        persistedBankAccount.Should().NotBeNull();
        persistedBankAccount!.Id.Should().Be(accountId);
        persistedBankAccount.Balance.Should().Be(40);
        persistedBankAccount.Version.Should().Be(3); // Initial + Deposit + Withdraw
    }

    [Fact]
    public async Task StoreAndLoad_WhenMultipleOperations_ShouldMaintainCorrectState()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        bankAccount.Deposit(Guid.CreateVersion7(), 1000, currency, meta);
        bankAccount.Deposit(Guid.CreateVersion7(), 50, currency, meta); 
        bankAccount.Withdrawn(500);
        bankAccount.Deposit(Guid.CreateVersion7(), 1, currency, meta);

        // Act
        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);
        var persistedBankAccount =
            await _eventStore.LoadAsync<BankAccountAggregate>(bankAccount.Id, null, CancellationToken.None);

        // Assert
        persistedBankAccount.Should().NotBeNull();
        persistedBankAccount!.Balance.Should().Be(551);
        persistedBankAccount.Version.Should().Be(5); // Initial + 4 operations
    }

    [Fact]
    public async Task Load_WhenAggregateDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act
        var result = await _eventStore.LoadAsync<BankAccountAggregate>(nonExistentId, null, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Load_WhenVersionSpecified_ShouldLoadCorrectVersion()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        bankAccount.Deposit(Guid.CreateVersion7(), 100, currency, meta);
        bankAccount.Deposit(Guid.CreateVersion7(), 200, currency, meta);
        bankAccount.Withdrawn(50);

        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Act - Load version 2 (after first deposit)
        var result = await _eventStore.LoadAsync<BankAccountAggregate>(accountId, 2, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Balance.Should().Be(100);
        result.Version.Should().Be(2);
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldHandleConflictsCorrectly()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var initialBankAccount = BankAccountAggregate.Open(accountId);
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        initialBankAccount.Deposit(Guid.CreateVersion7(), 1000, currency, meta);
        
        await _eventStore.StoreAsync(initialBankAccount, CancellationToken.None);

        // Act - Simulate concurrent operations
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Task.Run(async () =>
            {
                var bankAccount = await _eventStore.LoadAsync<BankAccountAggregate>(accountId, null, CancellationToken.None);
                
                bankAccount!.Deposit(Guid.CreateVersion7(), 100, currency, meta);
                
                await _eventStore.StoreAsync(bankAccount, CancellationToken.None);
            }))
            .ToList();

        // Assert - Some operations should fail due to concurrent updates
        var act = () => Task.WhenAll(tasks);
        await act.Should().ThrowAsync<ConcurrentUpdateException>();
    }

    [Fact]
    public async Task SequentialOperations_ShouldCompleteWithoutConflicts()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        bankAccount.Deposit(Guid.CreateVersion7(), 1000, currency, meta);
        
        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Act - Perform operations sequentially
        for (int i = 0; i < 5; i++)
        {
            var loadedBankAccount = await _eventStore.LoadAsync<BankAccountAggregate>(accountId, null, CancellationToken.None);
            
            loadedBankAccount.Should().NotBeNull();
            loadedBankAccount!.Deposit(Guid.CreateVersion7(), 100, currency, meta);
            
            await _eventStore.StoreAsync(loadedBankAccount, CancellationToken.None);
        }

        // Assert
        var finalBankAccount = await _eventStore.LoadAsync<BankAccountAggregate>(accountId, null, CancellationToken.None);
        finalBankAccount.Should().NotBeNull();
        finalBankAccount!.Balance.Should().Be(1500); // Initial 1000 + 5 deposits of 100 each
        finalBankAccount.Version.Should().Be(7); // Initial account (1) + first deposit (2) + 5 more deposits (3,4,5,6,7)
    }

    [Fact]
    public async Task Store_WhenAggregateHasNoUncommittedEvents_ShouldNotThrowException()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Act & Assert - Should not throw
        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);
    }

    [Fact]
    public async Task Store_WhenAggregateHasEvents_ShouldPersistAllEvents()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        bankAccount.Deposit(Guid.CreateVersion7(), 100, currency, meta);
        bankAccount.Withdrawn(30);

        // Act
        await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Assert
        var events = await _session.Events.FetchStreamAsync(accountId);
        events.Should().HaveCount(3); // Initial + Deposit + Withdraw
    }
    
    [Fact]
    public async Task Store_TwoEventsWithSameId_ShouldThrow()
    {
        // Arrange
        var accountId = Guid.CreateVersion7();
        var bankAccount = BankAccountAggregate.Open(accountId);
        var invoiceId = Guid.CreateVersion7();
        var currency = _faker.Random.String2(3);
        var meta = new Faker<Payment>().Generate();
        
        bankAccount.Deposit(invoiceId, 100, currency, meta);
        bankAccount.Deposit(invoiceId, 100, currency, meta);

        // Act
        Func<Task> func = async () => await _eventStore.StoreAsync(bankAccount, CancellationToken.None);

        // Assert
        await func.Should().ThrowAsync<Exception>();
    }
}