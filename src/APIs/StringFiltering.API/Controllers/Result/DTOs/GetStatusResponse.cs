namespace StringFiltering.API.Controllers.Result.DTOs;

public class GetStatusResponse
{
    public required string UploadId { get; set; }
    public required string Status { get; set; }
}