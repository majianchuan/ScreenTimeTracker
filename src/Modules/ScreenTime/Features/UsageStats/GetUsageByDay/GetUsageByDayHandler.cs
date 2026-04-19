using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageByDay;

public class GetUsageByDayHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetUsageByDayQuery, List<GetUsageByDayResponseItem>>
{
    public async ValueTask<List<GetUsageByDayResponseItem>> Handle(GetUsageByDayQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayBoundaryOffsetHours);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayBoundaryOffsetHours);
        bool isAppCategory = request.Dimension == "app-category";

        var sessions = await context.AppUsageSessions
            .Where(x =>
                (isAppCategory
                    ? x.App!.AppCategoryId == request.Id
                    : x.AppId == request.Id)
                && x.StartTime < endTime
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
        var dailyUsage = new Dictionary<DateOnly, long>();
        for (var day = request.StartDate; day <= request.EndDate; day = day.AddDays(1))
            dailyUsage[day] = 0;

        foreach (var session in sessions)
        {
            DateOnly sessionStart = DateOnly.FromDateTime(session.StartTime.AddHours(-settings.DayBoundaryOffsetHours));
            DateOnly sessionEnd = DateOnly.FromDateTime(session.EndTime.AddHours(-settings.DayBoundaryOffsetHours));

            // 取交集
            DateOnly loopStart = sessionStart < request.StartDate ? request.StartDate : sessionStart;
            DateOnly loopEnd = request.EndDate < sessionEnd ? request.EndDate : sessionEnd;

            for (var day = loopStart; day <= loopEnd; day = day.AddDays(1))
            {
                DateTime dayStart = day.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayBoundaryOffsetHours);
                DateTime dayEnd = day.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayBoundaryOffsetHours);

                // 时间段的交集
                DateTime actualStart = dayStart < session.StartTime ? session.StartTime : dayStart;
                DateTime actualEnd = session.EndTime < dayEnd ? session.EndTime : dayEnd;
                // 如果实际开始时间早于实际结束时间，说明在这一天有有效交集
                if (actualStart < actualEnd)
                    dailyUsage[day] += (long)(actualEnd - actualStart).TotalMilliseconds;
            }
        }

        // 使用时长精度保留到秒
        return [.. dailyUsage
            .Select(kvp => new GetUsageByDayResponseItem(kvp.Key, kvp.Value / 1000))
            .OrderBy(x => x.Date)];
    }
}