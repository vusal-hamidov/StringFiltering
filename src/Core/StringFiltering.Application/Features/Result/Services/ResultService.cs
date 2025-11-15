using StringFiltering.Application.Features.Result.Models;
using StringFiltering.Application.Interfaces;

namespace StringFiltering.Application.Features.Result.Services;

public class ResultService(IResultStore store) : IResultService
{
    public GetStatusOutput GetStatus(string uploadId)
    {
        return new GetStatusOutput
        {
            UploadId = uploadId,
            Status = store.GetStatus(uploadId)
        };
    }

    public GetResultOutput GetResult(string uploadId)
    {
        var text = store.GetResult(uploadId);
        return new GetResultOutput
        {
            UploadId = uploadId,
            Status = text != null ? "Completed" : "Processing or NotFound",
            FilteredText = text
        };
    }
}