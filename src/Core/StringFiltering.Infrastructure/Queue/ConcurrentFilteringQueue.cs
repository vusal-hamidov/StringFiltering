using System.Collections.Concurrent;
using StringFiltering.Application.Interfaces;

namespace StringFiltering.Infrastructure.Queue;

public sealed class ConcurrentFilteringQueue : IFilteringQueue, IDisposable
{
    private readonly ConcurrentQueue<(string UploadId, string Text)> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private bool _disposed;

    public void Enqueue(string uploadId, string text)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _queue.Enqueue((uploadId, text));
        _signal.Release();
    }

    public async Task<(string UploadId, string Text)?> DequeueAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);
        _queue.TryDequeue(out var item);
        return item;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _signal.Dispose();

        _disposed = true;
    }
}