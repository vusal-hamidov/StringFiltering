using Moq;
using StringFiltering.Application.Interfaces;
using StringFiltering.Infrastructure.Background;

namespace StringFiltering.UnitTests.Background;

public class FilteringBackgroundServiceTests
{
    private readonly Mock<IFilteringQueue> _queueMock = new();
    private readonly Mock<ITextFilter> _filterMock = new();
    private readonly Mock<IResultStore> _storeMock = new();
    private FilteringBackgroundService CreateService() => new(_queueMock.Object, _filterMock.Object, _storeMock.Object);

    [Fact]
    public async Task ExecuteAsyncProcessesSingleTaskSuccessfully()
    {
        // Arrange
        const string uploadId = "AZ-001";
        const string text = "Bu bizim maşındır";
        const string filteredText = "Bu maşındır";
        using var cancellationTokenSource = new CancellationTokenSource();

        var dequeueCalls = 0;
        _queueMock
            .Setup(queue => queue.DequeueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                dequeueCalls++;
                return dequeueCalls == 1 ? (uploadId, text) : null;
            });

        _filterMock
            .Setup(f => f.Filter(text))
            .Returns(filteredText);

        using var service = CreateService();

        // Act
        var execTask = service.StartAsync(cancellationTokenSource.Token);

        await Task.Delay(100, cancellationTokenSource.Token);

        await cancellationTokenSource.CancelAsync();
        await execTask;

        // Assert
        _storeMock.Verify(s => s.SetProcessing(uploadId), Times.Once);
        _filterMock.Verify(f => f.Filter(text), Times.Once);
        _storeMock.Verify(s => s.SetCompleted(uploadId, filteredText), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsyncStopsOnCancellation()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        
        using var service = CreateService();

        // Act (service should exit immediately)
        await service.StartAsync(cancellationTokenSource.Token);

        // Assert
        _queueMock.Verify(q => q.DequeueAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsyncContinuesOnException()
    {
        // Arrange
        const string uploadId = "AZ-002";
        const string text = "Böyük mətn...";
        using var cancellationTokenSource = new CancellationTokenSource();
    
#pragma warning disable IDISP013
        _queueMock.SetupSequence(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
#pragma warning restore IDISP013
            .ReturnsAsync((uploadId, text))
            .ReturnsAsync(((string UploadId, string Text)?)null);

        _filterMock
            .Setup(f => f.Filter(text))
            .Throws(new InvalidOperationException("Filter error"));

        using var service = CreateService();
    
        // Act
        var execTask = service.StartAsync(cancellationTokenSource.Token);
        
        await Task.Delay(100, cancellationTokenSource.Token);
    
        await cancellationTokenSource.CancelAsync();
        await execTask;
    
        // Assert
        _storeMock.Verify(s => s.SetProcessing(uploadId), Times.Once);
        _storeMock.Verify(s => s.SetCompleted(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}