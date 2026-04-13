using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Features.UserPreferencesManagement.GetUserSettings;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.AggregateAppUsage;

public class AggregateAppUsageJob(
    ILogger<AggregateAppUsageJob> logger,
    IServiceScopeFactory scopeFactory
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("AggregateAppUsageJob is starting.");

        var activeInterval = await GetAggregationIntervalAsync(cancellationToken);
        PeriodicTimer timer = new(activeInterval);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 先聚合，再等待下一个间隔
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new AggregateAppUsageCommand(), cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while aggregating app usage.");
                }
                await timer.WaitForNextTickAsync(cancellationToken);
                // 获取最新间隔
                var latestInterval = await GetAggregationIntervalAsync(cancellationToken);
                if (latestInterval != activeInterval)
                {
                    activeInterval = latestInterval;

                    // 取消旧 timer 并重新创建
                    timer.Dispose();
                    timer = new PeriodicTimer(activeInterval);
                }
            }
        }
        finally
        {
            timer?.Dispose();
        }
    }

    private async Task<TimeSpan> GetAggregationIntervalAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userSettings = await mediator.Send(new GetUserSettingsQuery(), cancellationToken);

        return userSettings.AggregationInterval;
    }
}
