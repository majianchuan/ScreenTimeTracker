using System.Text.Json;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ImportData;

public class ImportDataHandler(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    IActiveSessionStore activeSessionStore
    ) : IRequestHandler<ImportDataCommand, ImportDataResponse>
{
    private class VersionedData
    {
        public int Version { get; set; }
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async ValueTask<ImportDataResponse> Handle(ImportDataCommand request, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(request.RawJson);
        var version = doc.RootElement.GetProperty("version").GetInt32();
        var response = version switch
        {
            1 => await ImportDataV1(request.RawJson, cancellationToken),
            _ => throw new NotSupportedException($"未知版本: {version}")
        };
        return response;
    }

    private async ValueTask<ImportDataResponse> ImportDataV1(string rawJson, CancellationToken cancellationToken)
    {
        var data = JsonSerializer.Deserialize<DataV1>(rawJson, _jsonSerializerOptions)
            ?? throw new NotSupportedException("无法解析数据");
        if (data.UsageSessions.Length == 0)
            return new ImportDataResponse(0, 0);

        // 计算导入数据的整体时间范围
        var minStart = data.UsageSessions.Min(s => s.StartTime);
        var maxEnd = data.UsageSessions.Max(s => s.EndTime);

        // 一次性拉取范围内的已有记录和 App
        var existingSessions = await context.AppUsageSessions
            .Where(s => s.StartTime < maxEnd && minStart < s.EndTime)
            .ToListAsync(cancellationToken);
        var activeSession = activeSessionStore.Current;
        var now = timeProvider.GetLocalNow().DateTime;
        if (activeSession is not null && activeSession.StartTime < maxEnd)
            existingSessions.Add(AppUsageSession.Create(activeSession.AppId, activeSession.StartTime, now));
        var processNames = data.UsageSessions.Select(s => s.AppProcessName).Distinct().ToList();
        var existingApps = await context.Apps
            .Where(a => processNames.Contains(a.ProcessName))
            .ToListAsync(cancellationToken);

        var appCache = existingApps.ToDictionary(a => a.ProcessName);

        long importedCount = 0;
        long skippedCount = 0;
        foreach (var session in data.UsageSessions)
        {
            if (now <= session.EndTime)
            {
                skippedCount++;
                continue;
            }
            var hasOverlap = existingSessions
                .Any(s => s.StartTime < session.EndTime && session.StartTime < s.EndTime);
            if (hasOverlap)
            {
                skippedCount++;
                continue;
            }

            if (!appCache.TryGetValue(session.AppProcessName, out var app))
            {
                app = App.Create(DateTime.MinValue, session.AppName, session.AppProcessName);
                context.Apps.Add(app);
                appCache[session.AppProcessName] = app;
            }

            var usageSession = AppUsageSession.Create(app.Id, session.StartTime, session.EndTime);
            context.AppUsageSessions.Add(usageSession);
            // 加入内存列表，避免同批次数据互相重叠时漏检
            existingSessions.Add(usageSession);
            importedCount++;
        }

        await context.SaveChangesAsync(cancellationToken);
        return new ImportDataResponse(importedCount, skippedCount);
    }
}

public record DataV1(
    UsageSession[] UsageSessions
);

public record UsageSession(
    string AppName,
    string AppProcessName,
    DateTime StartTime,
    DateTime EndTime
);