using System.Collections.Concurrent;
using StringFiltering.Application.Interfaces;

namespace StringFiltering.Infrastructure.Storage;

public class InMemoryResultStore : IResultStore
{
    private sealed class UploadState
    {
        public string Status { get; set; } = "NotFound";
        public string? Result { get; set; }
    }

    private readonly ConcurrentDictionary<string, UploadState> _states = new();

    public void SetProcessing(string uploadId)
    {
        var state = _states.GetOrAdd(uploadId, _ => new UploadState());
        state.Status = "Processing";
    }

    public void SetCompleted(string uploadId, string result)
    {
        var state = _states.GetOrAdd(uploadId, _ => new UploadState());
        state.Status = "Completed";
        state.Result = result;
    }

    public string? GetResult(string uploadId) => _states.TryGetValue(uploadId, out var state) ? state.Result : null;
    public string GetStatus(string uploadId) => _states.TryGetValue(uploadId, out var state) ? state.Status : "NotFound";
}