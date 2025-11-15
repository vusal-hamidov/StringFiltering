namespace StringFiltering.API.Controllers.Upload.DTOs;

public class UploadChunkResponse
{
    public required string Status { get; init; }
    public string? ResultId { get; init; }
}