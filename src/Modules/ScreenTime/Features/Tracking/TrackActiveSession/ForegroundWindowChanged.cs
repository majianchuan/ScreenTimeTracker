using System.Collections.Concurrent;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

public record ForegroundWindowChangedCommand(
    WindowInfo? WindowInfo
) : IRequest;


public class ForegroundWindowChangedHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    IExecutableMetadataProvider executableMetadataProvider,
    IMediator mediator,
    TimeProvider timeProvider) : IRequestHandler<ForegroundWindowChangedCommand>
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async ValueTask<Unit> Handle(ForegroundWindowChangedCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetLocalNow().DateTime;

        Guid appId;
        if (request.WindowInfo is null)
            appId = App.UnknownAppId;
        else
            appId = await IdentifyApp(now, request.WindowInfo, cancellationToken);

        if (activeSessionStore.Current is null)
        {
            activeSessionStore.Current = new ActiveSessionState(appId, now);
            return Unit.Value;
        }
        else if (activeSessionStore.Current.AppId == appId)
            return Unit.Value;
        else
        {
            await mediator.Send(new SaveActiveSessionCommand(), cancellationToken);
            activeSessionStore.Current = new ActiveSessionState(appId, now);
        }
        return Unit.Value;
    }


    public async Task<Guid> IdentifyApp(
        DateTime now,
        WindowInfo windowInfo,
        CancellationToken cancellationToken)
    {
        string processName = windowInfo.ProcessName;
        string? executablePath = windowInfo.ExecutablePath;

        var sem = _locks.GetOrAdd(processName, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync(cancellationToken);

        UserSettings settings = await context.UserSettings
            .AsNoTracking().SingleAsync(cancellationToken: cancellationToken);
        App? app = await context.Apps
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
            await context.SaveChangesAsync(cancellationToken);
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
                await context.SaveChangesAsync(cancellationToken);
            }
        }
        sem.Release();
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