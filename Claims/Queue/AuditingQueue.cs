using System.Collections.Concurrent;

namespace Claims.Queue
{
    public class AuditingQueue
    {
        private readonly ConcurrentQueue<QueueItem> _queue = new (); // Id, Operation
        private readonly SemaphoreSlim _semaphore = new (0);
        private readonly ILogger<AuditingQueue> _logger;

        public AuditingQueue(ILogger<AuditingQueue> logger)
        {
            _logger = logger;
        }

        public void AddAuditAction(QueueItem item)
        {
            _queue.Enqueue(item);
            _logger.LogInformation("Adding item to queue");
            _semaphore.Release();
        }

        public async ValueTask<QueueItem?> ReadAuditActionFromQueue(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            var success = _queue.TryDequeue(out var item);

            return success ? item : null;
        }
    }
}
