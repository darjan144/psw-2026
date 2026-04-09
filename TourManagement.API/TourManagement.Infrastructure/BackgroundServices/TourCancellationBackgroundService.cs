using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;

namespace TourManagement.Infrastructure.BackgroundServices;

public class TourCancellationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TourCancellationBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public TourCancellationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<TourCancellationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<TourCancellationProcessor>();
                await processor.ProcessCancellationsAsync(stoppingToken);
                _logger.LogInformation("Tour cancellation check completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during tour cancellation check");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
