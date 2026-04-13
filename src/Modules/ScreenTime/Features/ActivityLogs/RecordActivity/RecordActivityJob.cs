using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Features.UserPreferencesManagement.GetUserSettings;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.ActivityLogs.RecordActivity;

public class RecordActivityJob(
    ILogger<RecordActivityJob> logger,
     IServiceScopeFactory scopeFactory
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("RecordActivityJob is starting.");

        var activeInterval = await GetSamplingIntervalAsync(cancellationToken);
        PeriodicTimer timer = new(activeInterval);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 先记录，然后等待
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new RecordActivityCommand(activeInterval), cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while recording activity.");
                }
                await timer.WaitForNextTickAsync(cancellationToken);
                // 获取最新间隔
                var latestInterval = await GetSamplingIntervalAsync(cancellationToken);
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

    private async Task<TimeSpan> GetSamplingIntervalAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userSettings = await mediator.Send(new GetUserSettingsQuery(), cancellationToken);

        return userSettings.SamplingInterval;
    }
}
