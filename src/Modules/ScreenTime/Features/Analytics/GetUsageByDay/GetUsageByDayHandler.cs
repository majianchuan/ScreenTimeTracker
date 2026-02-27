using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByDay;

public class GetUsageByDayHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetUsageByDayQuery, List<GetUsageByDayResponseItem>>
{
    public async ValueTask<List<GetUsageByDayResponseItem>> Handle(GetUsageByDayQuery request, CancellationToken cancellationToken)
    {
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1);
        var isApp = request.Dimension == "app";

        // 1. 构建基础查询
        var hourlyUsageQuery = context.AppHourlyUsages.AsNoTracking().Where(h => startTime <= h.Hour && h.Hour < endTime);
        var logQuery = context.ActivityLogs.AsNoTracking().Where(a => startTime <= a.LoggedAt && a.LoggedAt < endTime);

        // 2. 根据维度应用过滤条件
        if (isApp)
        {
            hourlyUsageQuery = hourlyUsageQuery.Where(h => h.AppId == request.Id);
            logQuery = logQuery.Where(a => a.AppId == request.Id);
        }
        else
        {
            // EF Core 会自动处理 Join，不需要显式写 Include
            hourlyUsageQuery = hourlyUsageQuery.Where(h => h.App!.AppCategoryId == request.Id);
            logQuery = logQuery.Where(a => a.App!.AppCategoryId == request.Id);
        }

        // 3. 投影、合并并执行聚合
        var usageByDate = await hourlyUsageQuery
            .Select(h => new { Date = h.Hour.Date, h.DurationMilliseconds })
            .Concat(logQuery.Select(a => new { Date = a.LoggedAt.Date, a.DurationMilliseconds }))
            .GroupBy(u => u.Date)
            .Select(g => new
            {
                Date = DateOnly.FromDateTime(g.Key),
                DurationMilliseconds = g.Sum(u => u.DurationMilliseconds)
            })
            .ToDictionaryAsync(k => k.Date, v => v.DurationMilliseconds, cancellationToken);

        // 4. 生成连续日期结果（补 0 逻辑）
        int daysCount = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;

        return [.. Enumerable.Range(0, daysCount)
            .Select(i => request.StartDate.AddDays(i))
            .Select(date => new GetUsageByDayResponseItem(
                Date: date,
                DurationSeconds: usageByDate.GetValueOrDefault(date) / 1000
            ))];
    }
}