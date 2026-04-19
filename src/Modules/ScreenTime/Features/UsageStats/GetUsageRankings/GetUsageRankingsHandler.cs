using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageRankings;

public class GetUsageRankingsHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetUsageRankingsQuery, List<GetUsageRankingsResponseItem>>
{
    public async ValueTask<List<GetUsageRankingsResponseItem>> Handle(GetUsageRankingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayBoundaryOffsetHours);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayBoundaryOffsetHours);
        bool isAppCategory = request.Dimension == "app-category";
        var excludedIds = request.ExcludedIds?.ToHashSet() ?? [];

        var sessions = await context.AppUsageSessions
            .Where(x => x.StartTime < endTime && startTime <= x.EndTime)
            .Select(x => new SessionDto(
                x.AppId,
                x.App!.Name,
                x.App!.IconPath,
                x.App!.AppCategoryId,
                x.App.AppCategory!.Name,
                x.App.AppCategory!.IconPath,
                x.StartTime,
                x.EndTime
            ))
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null)
        {
            var activeApp = await context.Apps
                .Include(a => a.AppCategory)
                .AsNoTracking()
                .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);

            sessions.Add(new SessionDto(
                activeApp.Id,
                activeApp!.Name,
                activeApp!.IconPath,
                activeApp!.AppCategoryId,
                activeApp.AppCategory!.Name,
                activeApp.AppCategory!.IconPath,
                activeSession.StartTime,
                timeProvider.GetLocalNow().DateTime
            ));
        }

        var aggregatedUsage = new Dictionary<Guid, (string Name, string? IconPath, long DurationMilliseconds)>();

        foreach (var session in sessions)
        {
            Guid targetId = isAppCategory ? (session.AppCategoryId ?? Guid.Empty) : session.AppId;

            if (excludedIds.Contains(targetId))
                continue;

            // 截取和请求时间段交集的时间 (避免多算范围外的时长)
            DateTime actualStart = startTime < session.StartTime ? session.StartTime : startTime;
            DateTime actualEnd = session.EndTime < endTime ? session.EndTime : endTime;

            if (actualStart < actualEnd)
            {
                long durationMilliseconds = (long)(actualEnd - actualStart).TotalMilliseconds;

                if (!aggregatedUsage.TryGetValue(targetId, out var current))
                {
                    // 若无分类，给个默认的兜底文本
                    string name = isAppCategory ? (session.AppCategoryName ?? "未分类") : session.AppName;
                    string? icon = isAppCategory ? session.AppCategoryIconPath : session.AppIconPath;

                    aggregatedUsage[targetId] = (name, icon, durationMilliseconds);
                }
                else
                {
                    aggregatedUsage[targetId] = (current.Name, current.IconPath, current.DurationMilliseconds + durationMilliseconds);
                }
            }
        }

        // 4. 计算总时长，用于计算百分比
        long totalDurationMsAll = aggregatedUsage.Values.Sum(x => x.DurationMilliseconds);

        // 5. 排序、格式化并取前 TopN 返回
        return [.. aggregatedUsage
            .Select(kvp => new GetUsageRankingsResponseItem(
                Id: kvp.Key,
                Name: kvp.Value.Name,
                IconPath: kvp.Value.IconPath,
                DurationSeconds: kvp.Value.DurationMilliseconds / 1000,
                // 计算百分比并四舍五入
                Percentage: totalDurationMsAll == 0 ? 0 : (int)Math.Round((double)kvp.Value.DurationMilliseconds / totalDurationMsAll * 100)
            ))
            // 按照使用时长倒序排列
            .OrderByDescending(x => x.DurationSeconds)
            // 取前 TopN
            .Take(request.TopN)];
    }

    private record SessionDto(
        Guid AppId,
        string AppName,
        string? AppIconPath,
        Guid? AppCategoryId,
        string AppCategoryName,
        string? AppCategoryIconPath,
        DateTime StartTime,
        DateTime EndTime
    );
}