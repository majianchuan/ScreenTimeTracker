using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.ActivityLogs.RecordActivity;

public class RecordActivityHandler(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    IIdleTimeProvider idleTimeProvider,
    IForegroundWindowService foregroundWindowService,
    IExecutableMetadataProvider executableMetadataProvider
    ) : IRequestHandler<RecordActivityCommand>
{
    public async ValueTask<Unit> Handle(RecordActivityCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetLocalNow().DateTime;
        TimeSpan idleTime = await idleTimeProvider.GetSystemIdleTimeAsync();
        UserSettings userSettings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        ActivityLog? newActivityLog;

        // 启用空闲检查并且空闲时间超过阈值，标记为空闲进程
        if (userSettings.IdleDetection && idleTime >= userSettings.IdleTimeout)
        {
            Guid idleAppId = await GetIdleAppId(now, cancellationToken);
            // 修正空闲开始以来的使用记录为空闲进程
            var idleStartTime = now.Add(-idleTime);
            var logsToUpdate = await context.ActivityLogs
                .Where(x => idleStartTime <= x.LoggedAt && x.LoggedAt < now && x.AppId != idleAppId)
                .ToListAsync(cancellationToken);
            foreach (var log in logsToUpdate)
                log.MarkAsIdle(idleAppId);

            // 新的也是空闲进程
            newActivityLog = ActivityLog.Create(idleAppId, now, request.Interval);
        }
        // 不是空闲，获取顶层窗口信息
        else
        {
            WindowInfo? windowInfo = foregroundWindowService.GetForegroundWindowInfo();
            if (windowInfo is null)
            {
                Guid unknownAppId = await GetUnknownAppId(now, cancellationToken);
                newActivityLog = ActivityLog.Create(unknownAppId, now, request.Interval);
            }
            else
            {
                Guid appId = await IdentifyApp(now, windowInfo.ProcessName, windowInfo.ExecutablePath, userSettings, cancellationToken);
                newActivityLog = ActivityLog.Create(appId, now, request.Interval);
            }
        }

        if (newActivityLog is not null)
            context.ActivityLogs.Add(newActivityLog);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    public async Task<Guid> GetIdleAppId(DateTime now, CancellationToken cancellationToken)
    {
        App? idleApp = await context.Apps
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == App.IdleAppId, cancellationToken: cancellationToken);
        if (idleApp is null)
        {
            idleApp = App.CreateIdleApp(now);
            context.Apps.Add(idleApp);
        }
        return idleApp.Id;
    }

    public async Task<Guid> GetUnknownAppId(DateTime now, CancellationToken cancellationToken)
    {
        App? unknownApp = await context.Apps
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == App.UnknownAppId, cancellationToken: cancellationToken);
        if (unknownApp is null)
        {
            unknownApp = App.CreateUnknownApp(now);
            context.Apps.Add(unknownApp);
        }
        return unknownApp.Id;
    }

    public async Task<Guid> IdentifyApp(
        DateTime now,
        string processName,
        string? executablePath,
        UserSettings settings,
        CancellationToken cancellationToken)
    {
        var app = await context.Apps
            .FirstOrDefaultAsync(p => p.ProcessName == processName, cancellationToken);

        // 新增 App 信息
        if (app is null)
        {
            if (executablePath is null)
                app = App.Create(now, processName, processName, true, executablePath);
            else
            {
                using ExecutableMetadata metadata = await executableMetadataProvider.GetMetadataAsync(executablePath);
                string name = string.IsNullOrWhiteSpace(metadata.Description) ? processName : metadata.Description;
                app = App.Create(now, name, processName, true, executablePath);
                string? iconPath = await EnsureIconUpdated(app, metadata, settings, cancellationToken);
                app.UpdateSystemDetails(now, executablePath, iconPath, metadata.Description);
            }
            context.Apps.Add(app);
        }
        // 已有 App 信息
        else
        {
            if (app.NeedsUpdate(now, settings.AppInfoStaleThreshold))
            {
                if (executablePath is null)
                    app.UpdateSystemDetails(now, executablePath, null, null);
                else
                {
                    using ExecutableMetadata metadata = await executableMetadataProvider.GetMetadataAsync(executablePath);
                    string? iconPath = await EnsureIconUpdated(app, metadata, settings, cancellationToken);
                    app.UpdateSystemDetails(now, executablePath, iconPath, metadata.Description);
                }
            }
        }

        return app.Id;
    }

    private static async Task<string?> EnsureIconUpdated(App app, ExecutableMetadata metadata, UserSettings settings, CancellationToken cancellationToken)
    {
        if (metadata.IconStream == null)
        {
            if (File.Exists(app.IconPath))
                File.Delete(app.IconPath);
            return null;
        }

        // 如果新旧图标大小相同，跳过 IO 操作（简单校验）
        if (File.Exists(app.IconPath) && new FileInfo(app.IconPath).Length == metadata.IconStream.Length)
        {
            return app.IconPath;
        }

        // 确保目录存在
        Directory.CreateDirectory(settings.AppIconDirectory);

        string newIconPath = Path.Combine(settings.AppIconDirectory, $"{app.ProcessName}.{metadata.IconFileExtension}");

        // 使用 FileMode.Create 自动覆盖旧文件
        await using (var fileStream = new FileStream(newIconPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await metadata.IconStream.CopyToAsync(fileStream, cancellationToken);
        }

        return newIconPath;
    }
}

public interface IIdleTimeProvider
{
    Task<TimeSpan> GetSystemIdleTimeAsync();
}

public interface IForegroundWindowService
{
    WindowInfo? GetForegroundWindowInfo();
}

public record WindowInfo(string ProcessName, string? ExecutablePath);

public interface IExecutableMetadataProvider
{
    public Task<ExecutableMetadata> GetMetadataAsync(string executablePath);
}

public class ExecutableMetadata(
    string? description,
    Stream? iconStream,
    string? iconFileExtension
) : IDisposable
{
    public string? Description { get; } = description;
    public Stream? IconStream { get; } = iconStream;
    public string? IconFileExtension { get; } = iconFileExtension;

    public void Dispose()
    {
        IconStream?.Dispose();
        GC.SuppressFinalize(this);
    }
}
