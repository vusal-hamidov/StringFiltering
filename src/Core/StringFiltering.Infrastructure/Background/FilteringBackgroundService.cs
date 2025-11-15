using Microsoft.Extensions.Hosting;
using StringFiltering.Application.Interfaces;

namespace StringFiltering.Infrastructure.Background;

public class FilteringBackgroundService(IFilteringQueue queue, ITextFilter filter, IResultStore store) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var task = await queue.DequeueAsync(stoppingToken);
                if (task == null)
                    continue;

                var (uploadId, text) = task.Value;
                store.SetProcessing(uploadId);

                var filtered = filter.Filter(text);
                store.SetCompleted(uploadId, filtered);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // FilteringBackgroundService stopping due to cancellation
                break;
            }
            catch
            {
                // Error processing filtering task
            }
        }
    }
}