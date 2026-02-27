using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByDay;

public class GetUsageSummaryByDayHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetUsageSummaryByDayQuery, List<GetUsageSummaryByDayResponseItem>>
{
    public async ValueTask<List<GetUsageSummaryByDayResponseItem>> Handle(GetUsageSummaryByDayQuery request, CancellationToken cancellationToken)
    {
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
        var endTime = request.EndDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        // 1. 构建基础查询
        var hourlyQuery = context.AppHourlyUsages.AsNoTracking().Where(h => startTime <= h.Hour && h.Hour < endTime);
        var logQuery = context.ActivityLogs.AsNoTracking().Where(l => startTime <= l.LoggedAt && l.LoggedAt < endTime);

        // 2. 统一处理排除过滤 (利用导航属性，无需显式 Include)
        if (request.ExcludedIds?.Any() is true)
        {
            bool isApp = request.Dimension == "app";
            hourlyQuery = isApp ? hourlyQuery.Where(h => !request.ExcludedIds.Contains(h.AppId))
                                : hourlyQuery.Where(h => !request.ExcludedIds.Contains(h.App!.AppCategoryId));

            logQuery = isApp ? logQuery.Where(l => !request.ExcludedIds.Contains(l.AppId))
                             : logQuery.Where(l => !request.ExcludedIds.Contains(l.App!.AppCategoryId));
        }

        // 3. 投影、合并、聚合数据到字典
        var usageByDate = await hourlyQuery
            .Select(h => new { Date = h.Hour.Date, h.DurationMilliseconds })
            .Concat(logQuery.Select(l => new { Date = l.LoggedAt.Date, l.DurationMilliseconds }))
            .GroupBy(x => x.Date)
            .Select(g => new
            {
                Date = DateOnly.FromDateTime(g.Key),
                TotalMs = g.Sum(x => x.DurationMilliseconds)
            })
            .ToDictionaryAsync(x => x.Date, x => x.TotalMs, cancellationToken);

        // 4. 生成连续日期序列并补齐 0
        int daysCount = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;

        return [.. Enumerable.Range(0, daysCount)
        .Select(i => request.StartDate.AddDays(i))
        .Select(date => new GetUsageSummaryByDayResponseItem(
            Date: date,
            DurationSeconds: usageByDate.GetValueOrDefault(date) / 1000
        ))];
    }
}