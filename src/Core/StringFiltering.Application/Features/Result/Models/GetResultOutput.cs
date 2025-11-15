namespace StringFiltering.Application.Features.Result.Models;

public class GetResultOutput
{
    public required string UploadId { get; init; }
    public required string Status { get; init; }
    public string? FilteredText { get; init; }
}