using ES.Yoomoney.Core.Entities;
using Xunit;

namespace ES.Yoomoney.Tests.Unit.Core.Entities;

public class OutboxMessageTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithCorrectValues()
    {
        // Arrange
        var id = Guid.CreateVersion7();
        var type = "TestType";
        var content = "TestContent";
        var createdAt = DateTime.UtcNow;
        DateTime? processedAt = null;

        // Act
        var message = new OutboxMessage(id, type, content, createdAt, processedAt);

        // Assert
        Assert.Equal(id, message.Id);
        Assert.Equal(type, message.Type);
        Assert.Equal(content, message.Content);
        Assert.Equal(createdAt, message.CreatedAt);
        Assert.Null(message.ProcessedAt);
    }

    [Fact]
    public void Constructor_WithProcessedAt_ShouldCreateInstanceWithProcessedDate()
    {
        // Arrange
        var id = Guid.CreateVersion7();
        var type = "TestType";
        var content = "TestContent";
        var createdAt = DateTime.UtcNow;
        var processedAt = DateTime.UtcNow.AddHours(1);

        // Act
        var message = new OutboxMessage(id, type, content, createdAt, processedAt);

        // Assert
        Assert.Equal(id, message.Id);
        Assert.Equal(type, message.Type);
        Assert.Equal(content, message.Content);
        Assert.Equal(createdAt, message.CreatedAt);
        Assert.Equal(processedAt, message.ProcessedAt);
    }

    [Fact]
    public void Records_ShouldBeEqual_WhenPropertiesAreEqual()
    {
        // Arrange
        var id = Guid.CreateVersion7();
        var type = "TestType";
        var content = "TestContent";
        var createdAt = DateTime.UtcNow;
        DateTime? processedAt = null;

        // Act
        var message1 = new OutboxMessage(id, type, content, createdAt, processedAt);
        var message2 = new OutboxMessage(id, type, content, createdAt, processedAt);

        // Assert
        Assert.Equal(message1, message2);
        Assert.True(message1 == message2);
    }
} 