using StringFiltering.Application.Features.Upload.Models;

namespace StringFiltering.Application.Features.Upload.Services;

public interface IUploadService
{
    UploadChunkOutput UploadChunk(UploadChunkInput model);
}