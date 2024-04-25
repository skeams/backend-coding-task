using Claims.Auditing;

namespace Claims.Queue
{
    public class QueueHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<QueueHostedService> _logger;
        private Timer? _timer = null;
        private readonly AuditingQueue _queue;
        private readonly Auditer _auditer;

        public QueueHostedService(ILogger<QueueHostedService> logger, AuditingQueue queue, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _queue = queue;

            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
            _auditer = new Auditer(context);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);

            var item = await _queue.ReadAuditActionFromQueue();
            if (item != null)
            {
                _logger.LogInformation($"Performing audit action: {item.Id} {item.Action} {item.IsClaim}");

                if (item.IsClaim)
                {
                    _auditer.AuditClaim(item.Id, item.Action);
                }
                else
                {
                    _auditer.AuditCover(item.Id, item.Action);
                }
            }

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
