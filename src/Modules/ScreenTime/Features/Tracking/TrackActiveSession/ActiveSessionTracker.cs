using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

public class ActiveSessionTracker(
        ILogger<ActiveSessionTracker> logger,
        IServiceScopeFactory scopeFactory,
        IActiveSessionStore activeSessionStore,
        IForegroundWindowMonitor foregroundWindowMonitor,
        IIdleTimeProvider idleTimeProvider,
        TimeProvider timeProvider) : BackgroundService
{
    private DateTime? _idleStartedAt;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("ActiveSessionTracker is starting.");

        using (var scope = scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var windowInfo = foregroundWindowMonitor.GetForegroundWindow();
            await mediator.Send(new ForegroundWindowChangedCommand(windowInfo), cancellationToken);
        }

        foregroundWindowMonitor.ForegroundWindowChanged += OnForegroundWindowChanged;

        // 空闲检测循环
        var settings = await GetUserSettingsAsync(cancellationToken);
        using var timer = new PeriodicTimer(settings.IdleDetectionPollingInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var newSettings = await GetUserSettingsAsync(cancellationToken);
                if (newSettings.IdleDetectionPollingInterval != settings.IdleDetectionPollingInterval)
                    timer.Period = newSettings.IdleDetectionPollingInterval;

                // 后续要用到 settings 中的 IdleThreshold，这里一定更新
                settings = newSettings;

                var now = timeProvider.GetLocalNow().DateTime;
                var systemIdleTime = await idleTimeProvider.GetSystemIdleTimeAsync();
                // 处于空闲状态
                if (systemIdleTime >= settings.IdleThreshold)
                {
                    // 从活跃状态到空闲状态
                    if (_idleStartedAt is null)
                    {
                        _idleStartedAt = now - systemIdleTime;
                        logger.LogInformation("System has become idle. Idle started at {IdleStartedAt}.", _idleStartedAt);
                        using var scope = scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Send(new SystemBecameIdleCommand(_idleStartedAt.Value), cancellationToken);
                    }
                }
                // 处于活跃状态
                else
                {
                    // 从空闲状态到活跃状态
                    if (_idleStartedAt is not null)
                    {
                        _idleStartedAt = null;
                        logger.LogInformation("System has become active.");
                        using var scope = scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var windowInfo = foregroundWindowMonitor.GetForegroundWindow();
                        await mediator.Send(new ForegroundWindowChangedCommand(windowInfo), cancellationToken);
                    }
                }
            }
        }
        catch (OperationCanceledException) { }

        foregroundWindowMonitor.ForegroundWindowChanged -= OnForegroundWindowChanged;

        // 退出时保存当前会话数据
        if (activeSessionStore.Current is null) return;
        using (var scope = scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new SaveActiveSessionCommand(), cancellationToken);
            activeSessionStore.Current = null;
        }
    }

    private async void OnForegroundWindowChanged(object? sender, WindowInfo? windowInfo)
    {
        try
        {
            // 处于空闲状态时，忽略窗口变化
            if (_idleStartedAt is not null)
                return;

            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new ForegroundWindowChangedCommand(windowInfo));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling foreground window change.");
        }
    }

    private async Task<UserSettings> GetUserSettingsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        ScreenTimeDbContext context = scope.ServiceProvider.GetRequiredService<ScreenTimeDbContext>();
        var userSettings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        return userSettings;
    }
}

public interface IForegroundWindowMonitor
{
    WindowInfo? GetForegroundWindow();
    event EventHandler<WindowInfo?> ForegroundWindowChanged;
}

public interface IIdleTimeProvider
{
    Task<TimeSpan> GetSystemIdleTimeAsync();
}

