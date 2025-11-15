namespace StringFiltering.Application.Features.Upload.Models;

public class UploadChunkInput
{
    public required string UploadId { get; init; } 
    public required int ChunkIndex { get; init; }
    public required ReadOnlyMemory<char> Data { get; init; }
    public required bool IsLastChunk { get; init; }
}