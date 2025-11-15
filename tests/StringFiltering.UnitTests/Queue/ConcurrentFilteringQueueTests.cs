using StringFiltering.Infrastructure.Queue;

namespace StringFiltering.UnitTests.Queue;

public class ConcurrentFilteringQueueTests
{
    [Fact]
    public async Task EnqueueThenDequeueReturnsSameItem()
    {
        // Arrange
        const string uploadId = "AZ-001";
        const string text = "Burada böyük bir mətn var...";
        using var queue = new ConcurrentFilteringQueue();

        // Act
        queue.Enqueue(uploadId, text);
        var result = await queue.DequeueAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(uploadId, result.Value.UploadId);
        Assert.Equal(text, result.Value.Text);
    }
    
    [Fact]
    public async Task DequeueAsyncWaitsUntilEnqueueThenReturnsItem()
    {
        // Arrange
        const string uploadId = "AZ-002";
        const string text = "Burada böyük bir mətn var...";
        using var queue = new ConcurrentFilteringQueue();
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        var dequeueTask = queue.DequeueAsync(cancellationTokenSource.Token);
        
        // make sure DequeueAsync is waiting
        await Task.Delay(100, cancellationTokenSource.Token);  
        queue.Enqueue(uploadId, text);

        var result = await dequeueTask;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(uploadId, result.Value.UploadId);
        Assert.Equal(text, result.Value.Text);
    }

    [Fact]
    public async Task DequeueAsyncCancellationRequestedThrowsTaskCanceledException()
    {
        // Arrange
        using var queue = new ConcurrentFilteringQueue();
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
    
        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => queue.DequeueAsync(cancellationTokenSource.Token));
    }
    
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers", "IDISP017:Prefer using", Justification = "Test for operation after Dispose")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers", "IDISP016:Don't use disposed instance", Justification = "Test for operation after Dispose")]
    public void EnqueueAfterDisposeThrowsObjectDisposedException()
    {
        // Arrange
        var queue = new ConcurrentFilteringQueue();
        queue.Dispose(); 

        // Act
        var act = () => queue.Enqueue("id", "text");

        // Assert
        Assert.Throws<ObjectDisposedException>(() => act());
    }
    
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers", "IDISP017:Prefer using", Justification = "Test for operation after Dispose")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers", "IDISP016:Don't use disposed instance", Justification = "Test for operation after Dispose")]
    public Task DequeueAsyncAfterDisposeThrowsObjectDisposedException()
    {
        // Arrange
        var queue = new ConcurrentFilteringQueue();
        queue.Dispose();

        // Act & Assert
        return Assert.ThrowsAsync<ObjectDisposedException>(() => queue.DequeueAsync(CancellationToken.None));
    }
    
    [Fact]
    public async Task EnqueueAndDequeueMultipleItemsWorkCorrectly()
    {
        // Arrange
        using var queue = new ConcurrentFilteringQueue();
        var items = new[]
        {
            ("id1", "text1"),
            ("id2", "text2"),
            ("id3", "text3"),
        };
    
        // Act
        foreach (var item in items)
            queue.Enqueue(item.Item1, item.Item2);
    
        var results = new List<(string UploadId, string Text)>();
        for (var i = 0; i < items.Length; i++)
        {
            var result = await queue.DequeueAsync(CancellationToken.None);
            results.Add(result!.Value);
        }
    
        // Assert
        Assert.Equal(items, results);
    }
}