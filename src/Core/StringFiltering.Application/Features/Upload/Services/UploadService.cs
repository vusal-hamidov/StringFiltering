using StringFiltering.Application.Features.Upload.Models;
using StringFiltering.Application.Interfaces;

namespace StringFiltering.Application.Features.Upload.Services;

public class UploadService(IChunkStorage chunkStorage, IFilteringQueue queue) : IUploadService
{
    public UploadChunkOutput UploadChunk(UploadChunkInput model)
    {
        chunkStorage.AddChunk(model.UploadId, model.ChunkIndex, model.Data);

        if (!model.IsLastChunk)
        {
            return new UploadChunkOutput
            {
                Status = "Accepted"
            };
        }

        var fullText = chunkStorage.GetFullTextAndClear(model.UploadId);
        if (fullText != null)
        {
            queue.Enqueue(model.UploadId, fullText);
        }

        return new UploadChunkOutput
        {
            Status = "Accepted", 
            ResultId = model.UploadId
        };
    }
}