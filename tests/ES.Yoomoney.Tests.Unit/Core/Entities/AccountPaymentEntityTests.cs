using ES.Yoomoney.Core.Entities;

using Xunit;

namespace ES.Yoomoney.Tests.Unit.Core.Entities;

public class AccountPaymentEntityTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithCorrectValues()
    {
        // Arrange
        var id = "TestId";
        var accountId = Guid.CreateVersion7();

        // Act
        var entity = new AccountPaymentEntity(id, accountId);

        // Assert
        Assert.Equal(id, entity.Id);
        Assert.Equal(accountId, entity.AccountId);
        Assert.False(entity.IsProcessed);
    }

    [Fact]
    public void SetIsProcessed_ShouldSetIsProcessedToTrue()
    {
        // Arrange
        var entity = new AccountPaymentEntity("TestId", Guid.CreateVersion7());

        // Act
        entity.SetIsProcessed();

        // Assert
        Assert.True(entity.IsProcessed);
    }

    [Fact]
    public void Records_ShouldBeEqual_WhenPropertiesAreEqual()
    {
        // Arrange
        var id = "TestId";
        var accountId = Guid.CreateVersion7();

        // Act
        var entity1 = new AccountPaymentEntity(id, accountId);
        var entity2 = new AccountPaymentEntity(id, accountId);

        // Assert
        Assert.Equal(entity1, entity2);
        Assert.True(entity1 == entity2);
    }
}