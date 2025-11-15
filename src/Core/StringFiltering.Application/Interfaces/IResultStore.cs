namespace StringFiltering.Application.Interfaces;

public interface IResultStore
{
    void SetProcessing(string uploadId);
    void SetCompleted(string uploadId, string result);
    string? GetResult(string uploadId);
    string GetStatus(string uploadId);
}