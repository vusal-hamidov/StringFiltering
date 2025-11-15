namespace StringFiltering.Application.Features.Result.Models;

public class GetStatusOutput
{
    public required string UploadId { get; init; }
    public required string Status { get; init; }
}