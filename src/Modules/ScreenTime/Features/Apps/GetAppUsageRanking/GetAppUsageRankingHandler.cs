using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageRanking;

public class GetAppUsageRankingHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetAppUsageRankingQuery, List<GetAppUsageRankingResponseItem>>
{
    public async ValueTask<List<GetAppUsageRankingResponseItem>> Handle(GetAppUsageRankingQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayCutoffHour);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayCutoffHour);
        var excludedIds = request.ExcludedIds?.ToHashSet() ?? [];

        var sessions = await context.AppUsageSessions
            .Where(x =>
                !excludedIds.Contains(x.AppId)
                && x.StartTime < endTime
                && startTime <= x.EndTime)
            .Select(x => new
            {
                Id = x.AppId,
                x.App!.Name,
                x.App!.IconPath,
                x.StartTime,
                x.EndTime
            })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null && !excludedIds.Contains(activeSession.AppId))
        {
            var activeApp = await context.Apps
                .AsNoTracking()
                .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);

            sessions.Add(new
            {
                activeApp.Id,
                activeApp.Name,
                activeApp.IconPath,
                activeSession.StartTime,
                EndTime = timeProvider.GetLocalNow().DateTime
            });
        }

        var aggregatedUsage = new Dictionary<Guid, (string Name, string? IconPath, long DurationMilliseconds)>();

        foreach (var session in sessions)
        {
            // 截取和请求时间段交集的时间 (避免多算范围外的时长)
            DateTime actualStart = startTime < session.StartTime ? session.StartTime : startTime;
            DateTime actualEnd = session.EndTime < endTime ? session.EndTime : endTime;

            if (actualStart < actualEnd)
            {
                long durationMilliseconds = (long)(actualEnd - actualStart).TotalMilliseconds;

                if (!aggregatedUsage.TryGetValue(session.Id, out var current))
                    aggregatedUsage[session.Id] = (session.Name, session.IconPath, durationMilliseconds);
                else
                    aggregatedUsage[session.Id] = (current.Name, current.IconPath, current.DurationMilliseconds + durationMilliseconds);
            }
        }

        // 计算总时长，用于计算百分比
        long totalDurationMsAll = aggregatedUsage.Values.Sum(x => x.DurationMilliseconds);

        // 排序、格式化并取前 TopN 返回
        return [.. aggregatedUsage
            .Select(kvp => new GetAppUsageRankingResponseItem(
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
}