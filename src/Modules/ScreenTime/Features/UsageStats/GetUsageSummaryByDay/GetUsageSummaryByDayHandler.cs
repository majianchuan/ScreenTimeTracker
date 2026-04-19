using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByDay;

public class GetUsageSummaryByDayHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetUsageSummaryByDayQuery, List<GetUsageSummaryByDayResponseItem>>
{
    public async ValueTask<List<GetUsageSummaryByDayResponseItem>> Handle(GetUsageSummaryByDayQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayBoundaryOffsetHours);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayBoundaryOffsetHours);

        bool isAppCategory = request.Dimension == "app-category";
        var excludedIds = request.ExcludedIds?.ToList() ?? [];
        bool hasExclusions = excludedIds.Count > 0;

        var sessions = await context.AppUsageSessions
            .Where(x =>
                (isAppCategory
                    ? !excludedIds.Contains(x.App!.AppCategoryId)
                    : !excludedIds.Contains(x.AppId))
                && x.StartTime < endTime
                && startTime <= x.EndTime)
            .Select(x => new { x.StartTime, x.EndTime })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null)
        {
            bool shouldAddActiveSession = true;

            if (hasExclusions)
            {
                if (isAppCategory)
                {
                    var app = await context.Apps.SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);
                    if (excludedIds.Contains(app.AppCategoryId))
                        shouldAddActiveSession = false;
                }
                else if (excludedIds.Contains(activeSession.AppId))
                    shouldAddActiveSession = false;
            }

            if (shouldAddActiveSession)
                sessions.Add(new { activeSession.StartTime, EndTime = timeProvider.GetLocalNow().DateTime });
        }

        // 初始化结果字典,使用时长的单位为Milliseconds
        var dailyUsage = new Dictionary<DateOnly, long>();
        for (var day = request.StartDate; day <= request.EndDate; day = day.AddDays(1))
            dailyUsage[day] = 0;

        // 计算交集并累加每日时长
        foreach (var session in sessions)
        {
            DateOnly sessionStart = DateOnly.FromDateTime(session.StartTime.AddHours(-settings.DayBoundaryOffsetHours));
            DateOnly sessionEnd = DateOnly.FromDateTime(session.EndTime.AddHours(-settings.DayBoundaryOffsetHours));

            // 获取会话与请求时间范围的日期交集
            DateOnly loopStart = sessionStart < request.StartDate ? request.StartDate : sessionStart;
            DateOnly loopEnd = request.EndDate < sessionEnd ? request.EndDate : sessionEnd;

            for (var day = loopStart; day <= loopEnd; day = day.AddDays(1))
            {
                DateTime dayStart = day.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayBoundaryOffsetHours);
                DateTime dayEnd = day.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayBoundaryOffsetHours);

                // 计算具体的时间段交集
                DateTime actualStart = dayStart < session.StartTime ? session.StartTime : dayStart;
                DateTime actualEnd = session.EndTime < dayEnd ? session.EndTime : dayEnd;

                // 如果实际开始早于实际结束，说明有有效使用时间
                if (actualStart < actualEnd)
                {
                    dailyUsage[day] += (long)(actualEnd - actualStart).TotalMilliseconds;
                }
            }
        }

        return [.. dailyUsage
            .Select(kvp => new GetUsageSummaryByDayResponseItem(
                Date: kvp.Key,
                DurationSeconds: kvp.Value / 1000
            ))
            .OrderBy(x => x.Date)];
    }
}