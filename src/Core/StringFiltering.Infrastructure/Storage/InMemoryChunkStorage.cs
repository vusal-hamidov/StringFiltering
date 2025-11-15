using System.Collections.Concurrent;
using System.Text;
using StringFiltering.Application.Interfaces;

namespace StringFiltering.Infrastructure.Storage;

public class InMemoryChunkStorage : IChunkStorage
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, ReadOnlyMemory<char>>> _storage = new();

    public void AddChunk(string uploadId, int index, ReadOnlyMemory<char> data)
    {
        var chunks = _storage.GetOrAdd(uploadId, _ => new ConcurrentDictionary<int, ReadOnlyMemory<char>>());
        chunks[index] = data;   
    }

    public string? GetFullTextAndClear(string uploadId)
    {
        if (!_storage.TryRemove(uploadId, out var chunks))
            return null;

        var sb = new StringBuilder();

        foreach (var key in chunks.Keys.Order())
        {
            sb.Append(chunks[key]);
        }

        return sb.ToString();
    }
}