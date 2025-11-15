using StringFiltering.API.Controllers.Upload.DTOs;
using StringFiltering.Application.Features.Upload.Models;

namespace StringFiltering.API.Controllers.Upload;

public static class UploadMapper
{
    public static UploadChunkInput MapToInput(this UploadChunkRequest chunkRequest)
    {
        return new UploadChunkInput
        {
            UploadId = chunkRequest.UploadId,
            ChunkIndex = chunkRequest.ChunkIndex,
            Data = chunkRequest.Data.AsMemory(),
            IsLastChunk = chunkRequest.IsLastChunk
        };
    }

    public static UploadChunkResponse MapToResponse(this UploadChunkOutput output)
    {
        return new UploadChunkResponse
        {
            Status = output.Status,
            ResultId = output.ResultId
        };
    }
}