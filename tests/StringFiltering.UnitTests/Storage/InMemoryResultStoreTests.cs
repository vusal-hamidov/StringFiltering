using StringFiltering.Infrastructure.Storage;

namespace StringFiltering.UnitTests.Storage;

public class InMemoryResultStoreTests
{
    private readonly InMemoryResultStore _store = new();

    [Fact]
    public void InitialStatusIsNotFound()
    {
        // Arrange
        const string uploadId = "AZ-001";

        // Act
        var status = _store.GetStatus(uploadId);
        var result = _store.GetResult(uploadId);

        // Assert
        Assert.Equal("NotFound", status);
        Assert.Null(result);
    }

    [Fact]
    public void SetProcessingUpdatesStatusToProcessing()
    {
        // Arrange
        const string uploadId = "AZ-002";

        // Act
        _store.SetProcessing(uploadId);

        // Assert
        Assert.Equal("Processing", _store.GetStatus(uploadId));
        Assert.Null(_store.GetResult(uploadId));
    }

    [Fact]
    public void SetCompletedUpdatesStatusAndResult()
    {
        // Arrange
        const string uploadId = "AZ-003";
        const string expectedResult = "filtered text";

        // Act
        _store.SetCompleted(uploadId, expectedResult);

        // Assert
        Assert.Equal("Completed", _store.GetStatus(uploadId));
        Assert.Equal(expectedResult, _store.GetResult(uploadId));
    }

    [Fact]
    public void SetProcessingThenSetCompletedUpdatesCorrectly()
    {
        // Arrange
        const string uploadId = "AZ-004";
        const string expectedResult = "final result";

        // Act
        _store.SetProcessing(uploadId);
        _store.SetCompleted(uploadId, expectedResult);

        // Assert
        Assert.Equal("Completed", _store.GetStatus(uploadId));
        Assert.Equal(expectedResult, _store.GetResult(uploadId));
    }
}