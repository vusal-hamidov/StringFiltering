namespace StringFiltering.Application.Features.Upload.Models;

public class UploadChunkOutput
{
    public required string Status { get; init; }
    public string? ResultId { get; init; }
}