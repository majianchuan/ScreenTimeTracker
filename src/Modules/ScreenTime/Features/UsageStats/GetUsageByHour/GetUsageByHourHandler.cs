using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByHour;

public class GetUsageByHourHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetUsageByHourQuery, List<GetUsageByHourResponseItem>>
{
    public async ValueTask<List<GetUsageByHourResponseItem>> Handle(GetUsageByHourQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.Date.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayBoundaryOffsetHours);
        var endTime = request.Date.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayBoundaryOffsetHours);
        bool isAppCategory = request.Dimension == "app-category";

        var sessions = await context.AppUsageSessions
            .Where(x =>
                (isAppCategory
                    ? x.App!.AppCategoryId == request.Id
                    : x.AppId == request.Id)
                && x.StartTime <= endTime
                && startTime <= x.EndTime)
            .Select(x => new { x.StartTime, x.EndTime })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null)
        {
            if (isAppCategory)
            {
                var activeApp = await context.Apps.SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);
                if (activeApp.AppCategoryId == request.Id)
                    sessions.Add(new { activeSession.StartTime, EndTime = timeProvider.GetLocalNow().DateTime });
            }
            else if (activeSession.AppId == request.Id)
                sessions.Add(new { activeSession.StartTime, EndTime = timeProvider.GetLocalNow().DateTime });
        }

        // 初始化结果字典，此时使用时长的单位是 Milliseconds
        // 这里的 key 是带有偏移量的相对小时数，例如 offset为4时，key 是 4 到 27
        var hourlyUsage = new Dictionary<int, long>();
        for (var hour = 0 + settings.DayBoundaryOffsetHours; hour <= 23 + settings.DayBoundaryOffsetHours; hour++)
            hourlyUsage[hour] = 0;

        foreach (var session in sessions)
        {
            // 逻辑上这一天固定有 24 个小时
            for (int i = 0; i < 24; i++)
            {
                int hourKey = i + settings.DayBoundaryOffsetHours;
                DateTime hourStart = request.Date.ToDateTime(TimeOnly.MinValue).AddHours(hourKey);
                DateTime hourEnd = hourStart.AddHours(1); // 每一个区间是 1 小时

                // 时间段的交集
                DateTime actualStart = hourStart < session.StartTime ? session.StartTime : hourStart;
                DateTime actualEnd = session.EndTime < hourEnd ? session.EndTime : hourEnd;

                // 如果实际开始时间早于实际结束时间，说明在这个小时有有效交集
                if (actualStart < actualEnd)
                {
                    hourlyUsage[hourKey] += (long)(actualEnd - actualStart).TotalMilliseconds;
                }
            }
        }

        // 使用时长精度保留到秒
        return [.. hourlyUsage
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new GetUsageByHourResponseItem(
                Hour: (kvp.Key % 24 + 24) % 24,
                DurationSeconds: kvp.Value / 1000
            ))
        ];
    }
}