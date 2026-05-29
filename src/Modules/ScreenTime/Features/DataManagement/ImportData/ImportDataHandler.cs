using System.Text.Json;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ImportData;

public class ImportDataHandler(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    IActiveSessionStore activeSessionStore)
    : IRequestHandler<ImportDataCommand, ImportDataResponse>
{
    public async ValueTask<ImportDataResponse> Handle(ImportDataCommand request, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(request.RawJson);
        var version = doc.RootElement.GetProperty("version").GetInt32();
        var response = version switch
        {
            1 => await new ImportDataV1Processor(context, timeProvider, activeSessionStore).ProcessAsync(request.RawJson, cancellationToken),
            2 => await new ImportDataV2Processor(context, timeProvider, activeSessionStore).ProcessAsync(request.RawJson, cancellationToken),
            _ => throw new NotSupportedException($"未知版本: {version}")
        };
        return response;
    }

}


public class ImportDataV1Processor(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    IActiveSessionStore activeSessionStore)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private record Data(Data.UsageSession[] UsageSessions)
    {
        public record UsageSession(
            string AppName,
            string AppProcessName,
            DateTime StartTime,
            DateTime EndTime
        );
    }

    public async ValueTask<ImportDataResponse> ProcessAsync(string rawJson, CancellationToken cancellationToken)
    {
        var data = JsonSerializer.Deserialize<Data>(rawJson, _jsonSerializerOptions)
            ?? throw new NotSupportedException("无法解析 V1 数据");
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
            // 信任同批次数据不会重叠
            // existingSessions.Add(usageSession);
            importedCount++;
        }

        await context.SaveChangesAsync(cancellationToken);
        return new ImportDataResponse(importedCount, skippedCount);
    }
}

public class ImportDataV2Processor(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    IActiveSessionStore activeSessionStore)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private record Data(
        Data.AppCategory[] AppCategories,
        Data.App[] Apps,
        Data.AppUsageSession[] AppUsageSessions)
    {
        public record Icon(string Extension, byte[] Data);

        public record AppCategory(string Name, Icon Icon);

        public record App(
            string Name,
            string ProcessName,
            string AppCategoryName,
            Icon Icon);

        public record AppUsageSession(
            string AppProcessName,
            DateTime StartTime,
            DateTime EndTime);
    };

    public async ValueTask<ImportDataResponse> ProcessAsync(string rawJson, CancellationToken cancellationToken)
    {
        var data = JsonSerializer.Deserialize<Data>(rawJson, _jsonSerializerOptions)
            ?? throw new NotSupportedException("无法解析 V2 数据");
        if (data.AppCategories.Length == 0 && data.Apps.Length == 0 && data.AppUsageSessions.Length == 0)
            return new ImportDataResponse(0, 0);

        var appIconDirectory = (await context.UserSettings.SingleAsync(cancellationToken)).AppIconDirectory;

        // 处理 AppCategory
        var existingCategories = await context.AppCategories
            .ToDictionaryAsync(c => c.Name, cancellationToken);

        foreach (var categoryData in data.AppCategories)
        {
            if (!existingCategories.TryGetValue(categoryData.Name, out var category))
            {
                var iconPath = await SaveIconAsync(categoryData.Icon, categoryData.Name, "./Data/AppCategoryIcons", cancellationToken);
                category = AppCategory.Create(
                    categoryData.Name,
                    iconPath);
                context.AppCategories.Add(category);
                existingCategories[categoryData.Name] = category;
            }
        }

        // 处理 Apps
        var existingApps = await context.Apps
            .ToDictionaryAsync(a => a.ProcessName, cancellationToken);

        foreach (var appData in data.Apps)
        {
            if (!existingApps.TryGetValue(appData.ProcessName, out var app))
            {
                var iconPath = await SaveIconAsync(appData.Icon, appData.Name, appIconDirectory, cancellationToken);

                app = App.Create(DateTime.MinValue, appData.Name, appData.ProcessName, iconPath: iconPath);

                existingCategories.TryGetValue(appData.AppCategoryName, out var matchedCategory);
                if (matchedCategory is null)
                {
                    matchedCategory = AppCategory.Create(appData.AppCategoryName);
                    context.AppCategories.Add(matchedCategory);
                    existingCategories[appData.AppCategoryName] = matchedCategory;
                }
                app.UpdateAppCategoryId(matchedCategory.Id);

                context.Apps.Add(app);
                existingApps[appData.ProcessName] = app;
            }
        }

        // 处理 AppUsageSessions
        if (data.AppUsageSessions.Length == 0)
            return new ImportDataResponse(0, 0);

        // 计算导入数据的整体时间范围
        var minStart = data.AppUsageSessions.Min(s => s.StartTime);
        var maxEnd = data.AppUsageSessions.Max(s => s.EndTime);

        // 一次性拉取范围内的已有记录和 App
        var existingSessions = await context.AppUsageSessions
            .Where(s => s.StartTime < maxEnd && minStart < s.EndTime)
            .ToListAsync(cancellationToken);
        var activeSession = activeSessionStore.Current;
        var now = timeProvider.GetLocalNow().DateTime;
        if (activeSession is not null && activeSession.StartTime < maxEnd)
            existingSessions.Add(AppUsageSession.Create(activeSession.AppId, activeSession.StartTime, now));
        var processNames = data.AppUsageSessions.Select(s => s.AppProcessName).Distinct().ToList();

        long importedCount = 0;
        long skippedCount = 0;
        foreach (var session in data.AppUsageSessions)
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

            if (!existingApps.TryGetValue(session.AppProcessName, out var app))
            {
                app = App.Create(DateTime.MinValue, session.AppProcessName, session.AppProcessName);
                context.Apps.Add(app);
                existingApps[session.AppProcessName] = app;
            }

            var usageSession = AppUsageSession.Create(app.Id, session.StartTime, session.EndTime);
            context.AppUsageSessions.Add(usageSession);
            // 信任同批次数据不会重叠
            // existingSessions.Add(usageSession);
            importedCount++;
        }

        await context.SaveChangesAsync(cancellationToken);
        return new ImportDataResponse(importedCount, skippedCount);
    }

    private static async Task<string?> SaveIconAsync(
        Data.Icon? icon,
        string iconName,
        string iconDirectory,
        CancellationToken cancellationToken)
    {
        if (icon is null || icon.Data is null || icon.Data.Length == 0)
            return null;

        try
        {
            if (!Directory.Exists(iconDirectory))
                Directory.CreateDirectory(iconDirectory);

            var fileName = $"{iconName}{icon.Extension}";
            var filePath = Path.Combine(iconDirectory, fileName);

            await File.WriteAllBytesAsync(filePath, icon.Data, cancellationToken);

            return filePath;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

