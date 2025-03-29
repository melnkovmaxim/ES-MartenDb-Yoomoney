using ES.Yoomoney.Common.Core.OperationResult;

namespace ES.Yoomoney.Tests.Unit.Core.OperationResult;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsError);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Fail_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var result = Result.Fail(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsError);
        Assert.Equal(errorMessage, result.Error);
    }
}