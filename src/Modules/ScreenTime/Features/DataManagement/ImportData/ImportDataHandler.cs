using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;
using System.Text.Json;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ImportData;

public class ImportDataHandler(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    IActiveAppUsageSessionStore activeSessionStore)
    : IRequestHandler<ImportDataCommand, ImportDataResponse>
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async ValueTask<ImportDataResponse> Handle(ImportDataCommand request, CancellationToken cancellationToken)
    {
        using var doc = JsonDocument.Parse(request.RawJson);
        var version = doc.RootElement.GetProperty("version").GetInt32();

        var v3Data = version switch
        {
            1 => UpcastV2ToV3(UpcastV1ToV2(Deserialize<ImportDataContracts.V1Data>(request.RawJson))),
            2 => UpcastV2ToV3(Deserialize<ImportDataContracts.V2Data>(request.RawJson)),
            3 => Deserialize<ImportDataContracts.V3Data>(request.RawJson),
            _ => throw new NotSupportedException($"未知版本: {version}")
        };

        return await SaveDataAsync(v3Data, cancellationToken);
    }

    private static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions) ?? throw new InvalidOperationException("JSON 解析失败");

    private static class ImportDataContracts
    {
        public record Icon(string Extension, byte[] Data);

        // V1 数据结构
        public record V1Data(V1UsageSession[] UsageSessions);
        public record V1UsageSession(string AppName, string AppProcessName, DateTime StartTime, DateTime EndTime);

        // V2 数据结构
        public record V2Data(V2AppCategory[] AppCategories, V2App[] Apps, V2AppUsageSession[] AppUsageSessions);
        public record V2AppCategory(string Name, Icon? Icon);
        public record V2App(string Name, string ProcessName, string AppCategoryName, Icon? Icon);
        public record V2AppUsageSession(string AppProcessName, DateTime StartTime, DateTime EndTime);

        // V3 数据结构
        public record V3Data(V3AppCategory[] AppCategories, V3App[] Apps, V3AppUsageSession[] AppUsageSessions);
        public record V3AppCategory(string Name, string Color, Icon? Icon);
        public record V3App(string Name, string Color, string ProcessName, bool IsAutoUpdateEnabled, string AppCategoryName, Icon? Icon);
        public record V3AppUsageSession(string AppProcessName, DateTime StartTime, DateTime EndTime);
    }

    private static ImportDataContracts.V2Data UpcastV1ToV2(ImportDataContracts.V1Data v1)
    {
        var apps = v1.UsageSessions
            .DistinctBy(s => s.AppProcessName)
            .Select(s => new ImportDataContracts.V2App(s.AppName, s.AppProcessName, "Uncategorized", null))
            .ToArray();

        var sessions = v1.UsageSessions
            .Select(s => new ImportDataContracts.V2AppUsageSession(s.AppProcessName, s.StartTime, s.EndTime))
            .ToArray();

        return new ImportDataContracts.V2Data([], apps, sessions);
    }

    private static ImportDataContracts.V3Data UpcastV2ToV3(ImportDataContracts.V2Data v2)
    {
        var categories = v2.AppCategories
            .Select(c => new ImportDataContracts.V3AppCategory(c.Name, GenerateColor(), c.Icon))
            .ToArray();

        var apps = v2.Apps
            .Select(a => new ImportDataContracts.V3App(a.Name, GenerateColor(), a.ProcessName, true, a.AppCategoryName, a.Icon))
            .ToArray();

        var sessions = v2.AppUsageSessions
            .Select(s => new ImportDataContracts.V3AppUsageSession(s.AppProcessName, s.StartTime, s.EndTime))
            .ToArray();

        return new ImportDataContracts.V3Data(categories, apps, sessions);
    }

    private async Task<ImportDataResponse> SaveDataAsync(ImportDataContracts.V3Data data, CancellationToken cancellationToken)
    {
        if (data.AppCategories.Length == 0 && data.Apps.Length == 0 && data.AppUsageSessions.Length == 0)
            return new ImportDataResponse(0, 0, 0, 0);

        long newAppCategories = 0;
        long newApps = 0;
        long importedSessions = 0;
        long skippedSessions = 0;

        DateTime now = timeProvider.GetLocalNow().DateTime;

        var appIconDirectory = (await context.UserSettings.SingleAsync(cancellationToken)).AppIconDirectory;

        // 处理 AppCategory
        var existingCategories = await context.AppCategories
            .ToDictionaryAsync(c => c.Name, cancellationToken);

        foreach (var categoryData in data.AppCategories)
        {
            if (!existingCategories.TryGetValue(categoryData.Name, out var category))
            {
                var iconPath = await SaveIconAsync(categoryData.Icon, categoryData.Name, "./Data/AppCategoryIcons", cancellationToken);
                category = AppCategory.Rehydrate(categoryData.Name, categoryData.Color, iconPath, now, false);
                context.AppCategories.Add(category);
                existingCategories[categoryData.Name] = category;
                newAppCategories++;
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
                app = App.Rehydrate(appData.Name, appData.Color, appData.ProcessName, appData.IsAutoUpdateEnabled, DateTime.MinValue, AppCategory.UncategorizedId, null, iconPath, now, false);

                existingCategories.TryGetValue(appData.AppCategoryName, out var matchedCategory);
                if (matchedCategory is null)
                {
                    matchedCategory = AppCategory.Rehydrate(appData.AppCategoryName, GenerateColor(), null, now, false);
                    context.AppCategories.Add(matchedCategory);
                    existingCategories[appData.AppCategoryName] = matchedCategory;
                    newAppCategories++;
                }
                app.UpdateAppCategoryId(matchedCategory.Id);

                context.Apps.Add(app);
                existingApps[appData.ProcessName] = app;
                newApps++;
            }
        }

        // 处理 AppUsageSessions
        var minStart = data.AppUsageSessions.Min(s => s.StartTime);
        var maxEnd = data.AppUsageSessions.Max(s => s.EndTime);

        var existingSessions = await context.AppUsageSessions
            .Where(s => s.StartTime < maxEnd && minStart < s.EndTime)
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null && activeSession.StartTime < maxEnd)
            existingSessions.Add(AppUsageSession.Rehydrate(activeSession.AppId, activeSession.StartTime, now));

        foreach (var session in data.AppUsageSessions)
        {
            if (now <= session.EndTime)
            {
                skippedSessions++;
                continue;
            }

            var hasOverlap = existingSessions
                .Any(s => s.StartTime < session.EndTime && session.StartTime < s.EndTime);
            if (hasOverlap)
            {
                skippedSessions++;
                continue;
            }

            if (!existingApps.TryGetValue(session.AppProcessName, out var app))
            {
                app = App.Rehydrate(session.AppProcessName, GenerateColor(), session.AppProcessName, true, DateTime.MinValue, AppCategory.UncategorizedId, null, null, now, false);
                context.Apps.Add(app);
                existingApps[session.AppProcessName] = app;
            }

            var usageSession = AppUsageSession.Rehydrate(app.Id, session.StartTime, session.EndTime, false);
            context.AppUsageSessions.Add(usageSession);
            importedSessions++;
        }

        await context.SaveChangesAsync(cancellationToken);
        return new ImportDataResponse(newAppCategories, newApps, importedSessions, skippedSessions);
    }

    private static async Task<string?> SaveIconAsync(ImportDataContracts.Icon? icon, string iconName, string iconDirectory, CancellationToken cancellationToken)
    {
        if (icon is null || icon.Data is null || icon.Data.Length == 0) return null;
        try
        {
            if (!Directory.Exists(iconDirectory)) Directory.CreateDirectory(iconDirectory);
            var filePath = Path.Combine(iconDirectory, $"{iconName}{icon.Extension}");
            await File.WriteAllBytesAsync(filePath, icon.Data, cancellationToken);
            return filePath;
        }
        catch { return null; }
    }

    private static string HslToHex(double h, double s, double l)
    {
        h %= 360;
        s /= 100.0;
        l /= 100.0;

        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        double m = l - c / 2;

        double r1 = 0, g1 = 0, b1 = 0;

        if (h < 60)
            (r1, g1, b1) = (c, x, 0);
        else if (h < 120)
            (r1, g1, b1) = (x, c, 0);
        else if (h < 180)
            (r1, g1, b1) = (0, c, x);
        else if (h < 240)
            (r1, g1, b1) = (0, x, c);
        else if (h < 300)
            (r1, g1, b1) = (x, 0, c);
        else
            (r1, g1, b1) = (c, 0, x);

        int r = (int)Math.Round((r1 + m) * 255);
        int g = (int)Math.Round((g1 + m) * 255);
        int b = (int)Math.Round((b1 + m) * 255);

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static string GenerateColor()
    {
        int h = Random.Shared.Next(360);
        int s = Random.Shared.Next(60, 100);
        int l = Random.Shared.Next(50, 80);
        return HslToHex(h, s, l);
    }
}

