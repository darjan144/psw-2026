using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;

namespace TourManagement.Infrastructure.BackgroundServices;

public class TourReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TourReminderBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public TourReminderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<TourReminderBackgroundService> logger)
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
                var processor = scope.ServiceProvider.GetRequiredService<TourReminderProcessor>();
                await processor.ProcessRemindersAsync(stoppingToken);
                _logger.LogInformation("Tour reminder check completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during tour reminder check");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
