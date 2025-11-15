namespace StringFiltering.API.Controllers.Result.DTOs;

public class GetResultResponse
{
    public required string UploadId { get; set; }
    public required string Status { get; set; }
    public string? FilteredText { get; set; }
}