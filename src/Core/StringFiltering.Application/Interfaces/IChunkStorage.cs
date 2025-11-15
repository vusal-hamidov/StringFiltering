namespace StringFiltering.Application.Interfaces;

public interface IChunkStorage
{
    void AddChunk(string uploadId, int index, ReadOnlyMemory<char> data);
    string? GetFullTextAndClear(string uploadId);
}