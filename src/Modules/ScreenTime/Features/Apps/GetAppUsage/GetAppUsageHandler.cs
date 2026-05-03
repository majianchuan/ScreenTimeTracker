using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public class GetAppUsageHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetAppUsageQuery, List<GetAppUsageResponseItem>>
{
    public async ValueTask<List<GetAppUsageResponseItem>> Handle(GetAppUsageQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayCutoffHour);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayCutoffHour);

        var query = context.AppUsageSessions
            .Where(x => x.StartTime < endTime && startTime <= x.EndTime);

        if (request.IncludedIds is not null)
            query = query.Where(x => request.IncludedIds.Contains(x.AppId));
        else if (request.ExcludedIds is not null)
            query = query.Where(x => !request.ExcludedIds.Contains(x.AppId));

        var sessions = await query
            .Select(x => new { x.StartTime, x.EndTime })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null)
        {
            var included = request.IncludedIds?.Contains(activeSession.AppId) ?? true;
            var notExcluded = request.ExcludedIds is null || !request.ExcludedIds.Contains(activeSession.AppId);
            var shouldInclude = request.IncludedIds is not null ? included : notExcluded;

            if (shouldInclude)
                sessions.Add(new { activeSession.StartTime, EndTime = timeProvider.GetLocalNow().DateTime });
        }
        // 初始化结果字典,使用时长的单位为Milliseconds
        var usage = new Dictionary<DateTime, long>();
        if (request.Granularity == "hour")
            for (var day = startTime; day < endTime; day = day.AddHours(1))
                usage[day] = 0;
        else if (request.Granularity == "day")
            for (var day = startTime; day < endTime; day = day.AddDays(1))
                usage[day] = 0;

        // 计算交集并累加每日时长
        foreach (var session in sessions)
        {
            var sessionStart = session.StartTime < startTime ? startTime : session.StartTime;
            var sessionEnd = endTime < session.EndTime ? endTime : session.EndTime;

            DateTime current;
            if (request.Granularity == "hour")
                current = new DateTime(sessionStart.Year, sessionStart.Month, sessionStart.Day, sessionStart.Hour, 0, 0);
            else if (request.Granularity == "day")
            {
                var temp = sessionStart.AddHours(-settings.DayCutoffHour);
                current = new DateTime(temp.Year, temp.Month, temp.Day, settings.DayCutoffHour, 0, 0);
            }
            if (request.Granularity == "hour")
            {
                // 对齐到整点
                var startHour = new DateTime(sessionStart.Year, sessionStart.Month, sessionStart.Day, sessionStart.Hour, 0, 0);

                for (var hour = startHour; hour < sessionEnd; hour = hour.AddHours(1))
                {
                    var bucketStart = hour;
                    var bucketEnd = hour.AddHours(1);

                    DateTime actualStart = bucketStart < sessionStart ? sessionStart : bucketStart;
                    DateTime actualEnd = sessionEnd < bucketEnd ? sessionEnd : bucketEnd;

                    if (actualStart < actualEnd)
                        usage[bucketStart] += (long)(actualEnd - actualStart).TotalMilliseconds;

                }
            }
            else if (request.Granularity == "day")
            {
                var temp = sessionStart.AddHours(-settings.DayCutoffHour);
                var startDay = new DateTime(temp.Year, temp.Month, temp.Day, settings.DayCutoffHour, 0, 0);

                for (var day = startDay; day < sessionEnd; day = day.AddDays(1))
                {
                    DateTime bucketStart = day;
                    DateTime bucketEnd = bucketStart.AddDays(1);

                    DateTime actualStart = bucketStart < sessionStart ? sessionStart : bucketStart;
                    DateTime actualEnd = sessionEnd < bucketEnd ? sessionEnd : bucketEnd;

                    if (actualStart < actualEnd)
                    {
                        var key = bucketStart;
                        usage[key] += (long)(actualEnd - actualStart).TotalMilliseconds;
                    }
                }
            }
        }

        // 使用时长精度保留到秒
        return [.. usage
            .Select(kvp => new GetAppUsageResponseItem(kvp.Key.ToString("O"), kvp.Value / 1000))
            .OrderBy(x => x.StartTime)];
    }
}