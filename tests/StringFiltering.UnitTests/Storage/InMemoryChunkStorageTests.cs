using StringFiltering.Infrastructure.Storage;

namespace StringFiltering.UnitTests.Storage;

public class InMemoryChunkStorageTests
{
    private readonly InMemoryChunkStorage _storage = new();

    [Fact]
    public void AddChunkThenGetFullTextAndClearReturnsConcatenatedTextInOrder()
    {
        // Arrange
        const string uploadId = "AZ-001";
        _storage.AddChunk(uploadId, 2, "Salam".AsMemory());
        _storage.AddChunk(uploadId, 0, "Dostum".AsMemory());
        _storage.AddChunk(uploadId, 1, ", ".AsMemory());

        // Act
        var result = _storage.GetFullTextAndClear(uploadId);

        // Assert
        Assert.Equal("Dostum, Salam", result);
        
        var afterClear = _storage.GetFullTextAndClear(uploadId);
        Assert.Null(afterClear);
    }

    [Fact]
    public void GetFullTextAndClearReturnsNullIfUploadIdNotExists()
    {
        // Act
        var result = _storage.GetFullTextAndClear("NonExistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AddChunkReplacesExistingChunkData()
    {
        // Arrange
        const string uploadId = "AZ-002";
        _storage.AddChunk(uploadId, 0, "Old".AsMemory());
        _storage.AddChunk(uploadId, 0, "New".AsMemory());

        // Act
        var result = _storage.GetFullTextAndClear(uploadId);

        // Assert
        Assert.Equal("New", result);
    }
}