using StringFiltering.API.Controllers.Result.DTOs;
using StringFiltering.Application.Features.Result.Models;

namespace StringFiltering.API.Controllers.Result;

public static class ResultMapper
{
    public static GetStatusResponse MapToResponse(this GetStatusOutput output)
    {
        return new GetStatusResponse
        {
            UploadId = output.UploadId,
            Status = output.Status
        };
    }

    public static GetResultResponse MapToResponse(this GetResultOutput output)
    {
        return new GetResultResponse
        {
            UploadId = output.UploadId,
            Status = output.Status,
            FilteredText = output.FilteredText
        };
    }
}