using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsageDistribution;

public class GetAppCategoryUsageDistributionHandler(
    ScreenTimeDbContext context,
    IActiveAppUsageSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetAppCategoryUsageDistributionQuery, GetAppCategoryUsageDistributionResponse>
{
    public async ValueTask<GetAppCategoryUsageDistributionResponse> Handle(GetAppCategoryUsageDistributionQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayCutoffHour);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayCutoffHour);
        var excludedIds = request.ExcludedIds?.ToHashSet() ?? [];

        var sessions = await context.AppUsageSessions
            .Where(x =>
                !excludedIds.Contains(x.App!.AppCategoryId)
                && x.StartTime < endTime
                && startTime <= x.EndTime)
            .Select(x => new
            {
                Id = x.App!.AppCategoryId,
                x.App!.AppCategory!.Name,
                x.App!.AppCategory!.Color,
                x.App.AppCategory!.IconPath,
                x.App.AppCategory!.IconLastUpdatedAt,
                x.StartTime,
                x.EndTime
            })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null)
        {
            var activeApp = await context.Apps
                .Include(a => a.AppCategory)
                .AsNoTracking()
                .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);

            if (!excludedIds.Contains(activeApp.AppCategoryId))
                sessions.Add(new
                {
                    Id = activeApp.AppCategoryId,
                    activeApp.AppCategory!.Name,
                    activeApp.AppCategory!.Color,
                    activeApp.AppCategory!.IconPath,
                    activeApp.AppCategory!.IconLastUpdatedAt,
                    activeSession.StartTime,
                    EndTime = timeProvider.GetLocalNow().DateTime
                });
        }

        var aggregatedUsage = new Dictionary<Guid, (string Name, string Color, string? IconPath, DateTime IconLastUpdatedAt, long DurationMilliseconds)>();

        foreach (var session in sessions)
        {
            // 截取和请求时间段交集的时间 (避免多算范围外的时长)
            DateTime actualStart = startTime < session.StartTime ? session.StartTime : startTime;
            DateTime actualEnd = session.EndTime < endTime ? session.EndTime : endTime;

            if (actualStart < actualEnd)
            {
                long durationMilliseconds = (long)(actualEnd - actualStart).TotalMilliseconds;

                if (!aggregatedUsage.TryGetValue(session.Id, out var current))
                    aggregatedUsage[session.Id] = (session.Name, session.Color, session.IconPath, session.IconLastUpdatedAt, durationMilliseconds);
                else
                    aggregatedUsage[session.Id] = (current.Name, current.Color, current.IconPath, current.IconLastUpdatedAt, current.DurationMilliseconds + durationMilliseconds);
            }
        }

        // 计算总体宏观指标
        int totalCount = aggregatedUsage.Count;
        // 先把毫秒转为秒，防止 topN 是全部时加和少于总时长
        long totalDurationSeconds = aggregatedUsage.Values.Sum(x => x.DurationMilliseconds / 1000);

        // 排序截取Top N列表
        var topNItems = aggregatedUsage
                .Select(kvp => new AppCategoryUsageDistributionItem(
                Id: kvp.Key,
                Name: kvp.Value.Name,
                Color: kvp.Value.Color,
                IconPath: kvp.Value.IconPath,
                IconLastUpdatedAt: kvp.Value.IconLastUpdatedAt,
                DurationSeconds: kvp.Value.DurationMilliseconds / 1000
            ))
            // 按照使用时长倒序排列
            .OrderByDescending(x => x.DurationSeconds)
            // 取前 TopN
            .Take(request.TopN)
            .ToList();

        // 倒推“其他”数据
        int othersCount = totalCount - topNItems.Count;
        long topNDurationSeconds = topNItems.Sum(x => x.DurationSeconds);
        long othersDurationSeconds = Math.Max(0, totalDurationSeconds - topNDurationSeconds);

        // 排序、格式化并取前 TopN 返回
        return new GetAppCategoryUsageDistributionResponse(
            Items: topNItems,
            TotalCount: totalCount,
            TotalDurationSeconds: totalDurationSeconds,
            OthersCount: othersCount,
            OthersDurationSeconds: othersDurationSeconds
        );
    }
}