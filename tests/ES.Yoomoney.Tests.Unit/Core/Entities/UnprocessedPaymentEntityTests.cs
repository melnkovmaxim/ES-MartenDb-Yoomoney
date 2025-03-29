using ES.Yoomoney.Core.Entities;
using Yandex.Checkout.V3;

namespace ES.Yoomoney.Tests.Unit.Core.Entities;

public class UnprocessedPaymentEntityTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithCorrectValues()
    {
        // Arrange
        var id = "TestId";
        var payment = new Payment
        {
            Id = "PaymentId",
            Amount = new Amount { Value = 100.00m, Currency = "RUB" }
        };

        // Act
        var entity = new UnprocessedPaymentEntity(id, payment);

        // Assert
        Assert.Equal(id, entity.Id);
        Assert.Equal(payment, entity.Payment);
    }

    [Fact]
    public void Records_ShouldBeEqual_WhenPropertiesAreEqual()
    {
        // Arrange
        var id = "TestId";
        var payment = new Payment
        {
            Id = "PaymentId",
            Amount = new Amount { Value = 100.00m, Currency = "RUB" }
        };

        // Act
        var entity1 = new UnprocessedPaymentEntity(id, payment);
        var entity2 = new UnprocessedPaymentEntity(id, payment);

        // Assert
        Assert.Equal(entity1, entity2);
        Assert.True(entity1 == entity2);
    }

    [Fact]
    public void Records_ShouldNotBeEqual_WhenPropertiesAreDifferent()
    {
        // Arrange
        var payment = new Payment
        {
            Id = "PaymentId",
            Amount = new Amount { Value = 100.00m, Currency = "RUB" }
        };

        // Act
        var entity1 = new UnprocessedPaymentEntity("Id1", payment);
        var entity2 = new UnprocessedPaymentEntity("Id2", payment);

        // Assert
        Assert.NotEqual(entity1, entity2);
        Assert.False(entity1 == entity2);
    }
}