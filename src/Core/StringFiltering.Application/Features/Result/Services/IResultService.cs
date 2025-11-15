using StringFiltering.Application.Features.Result.Models;

namespace StringFiltering.Application.Features.Result.Services;

public interface IResultService
{
    public GetStatusOutput GetStatus(string uploadId);
    public GetResultOutput GetResult(string uploadId);
}