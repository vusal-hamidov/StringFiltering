namespace StringFiltering.Application.Interfaces;

public interface IFilteringQueue
{
    void Enqueue(string uploadId, string text);
    Task<(string UploadId, string Text)?> DequeueAsync(CancellationToken cancellationToken);
}