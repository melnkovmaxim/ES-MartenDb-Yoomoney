using ES.Yoomoney.Common.Core.FunctionalResult;

namespace ES.Yoomoney.Tests.Unit.Core.OperationResult;

public class ResultTTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        var testValue = 42;

        // Act
        var result = Result<int>.Success(testValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsError);
        Assert.Null(result.Error);
        Assert.Equal(testValue, result.Value);
    }

    [Fact]
    public void Fail_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var result = Result<int>.Fail(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsError);
        Assert.Equal(errorMessage, result.Error);
        Assert.Equal(default, result.Value);
    }

    [Fact]
    public void Success_ShouldWorkWithReferenceTypes()
    {
        // Arrange
        var testValue = new TestClass { Id = 1, Name = "Test" };

        // Act
        var result = Result<TestClass>.Success(testValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsError);
        Assert.Null(result.Error);
        Assert.Equal(testValue, result.Value);
    }

    private sealed class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}