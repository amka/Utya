using System.Threading.Channels;
using Utya.Data;
using Utya.Models;

namespace Utya.Services;

public class ClickProcessor : IClickProcessor, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ClickProcessor> _logger;
    private readonly Channel<ClickTask> _channel;
    private Task? _processingTask;
    private CancellationTokenSource? _cts;

    public ClickProcessor(
        IServiceProvider services,
        ILogger<ClickProcessor> logger)
    {
        _services = services;
        _logger = logger;
        _channel = Channel.CreateUnbounded<ClickTask>();
    }

    public void EnqueueClick(Guid linkId, ClickData data)
    {
        _channel.Writer.TryWrite(new ClickTask(linkId, data));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _processingTask = ProcessQueueAsync(_cts.Token);
        return Task.CompletedTask;
    }

    private async Task ProcessQueueAsync(CancellationToken token)
    {
        _logger.LogInformation("ClickProcessor started");
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                await foreach (var task in _channel.Reader.ReadAllAsync(token))
                {
                    _logger.LogInformation("Processing click for {LinkId}", task.LinkId);
                    
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var click = new Click
                    {
                        ShortLinkId = task.LinkId,
                        IpAddress = task.Data.IpAddress,
                        UserAgent = task.Data.UserAgent,
                        Referrer = task.Data.Referrer,
                        CountryCode = task.Data.CountryCode,
                        Timestamp = DateTime.UtcNow
                    };

                    await dbContext.Clicks.AddAsync(click, token);
                    await dbContext.SaveChangesAsync(token);
                    
                    _logger.LogInformation("Click saved for {LinkId}", task.LinkId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки клика");
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Writer.Complete();
        await _cts?.CancelAsync()!;
        await (_processingTask ?? Task.CompletedTask);
    }

    public void Dispose() => _cts?.Dispose();

    private record ClickTask(Guid LinkId, ClickData Data);
}