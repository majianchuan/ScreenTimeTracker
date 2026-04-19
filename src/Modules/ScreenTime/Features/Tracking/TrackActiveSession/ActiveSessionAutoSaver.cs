using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

public class ActiveSessionAutoSaver(
    ILogger<ActiveSessionAutoSaver> logger,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("ActiveSessionFallbackSaver is starting.");

        var interval = await GetActiveSessionAutoSaveIntervalAsync(cancellationToken);
        using var timer = new PeriodicTimer(interval);
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var latestInterval = await GetActiveSessionAutoSaveIntervalAsync(cancellationToken);
                if (latestInterval != interval)
                {
                    interval = latestInterval;
                    timer.Period = interval;
                }

                using var scope = scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new SaveActiveSessionCommand(), cancellationToken);
            }
        }
        catch (OperationCanceledException) { }
    }

    private async Task<TimeSpan> GetActiveSessionAutoSaveIntervalAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        ScreenTimeDbContext context = scope.ServiceProvider.GetRequiredService<ScreenTimeDbContext>();
        var userSettings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        return userSettings.ActiveSessionAutoSaveInterval;
    }
}
