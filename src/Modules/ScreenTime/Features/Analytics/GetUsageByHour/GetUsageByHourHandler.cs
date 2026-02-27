using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageByHour;

public class GetUsageByHourHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetUsageByHourQuery, List<GetUsageByHourResponseItem>>
{
    public async ValueTask<List<GetUsageByHourResponseItem>> Handle(GetUsageByHourQuery request, CancellationToken cancellationToken)
    {
        var startTime = request.Date.ToDateTime(TimeOnly.MinValue);
        var endTime = startTime.AddDays(1);
        var isApp = request.Dimension == "app";

        // 1. 构建基础查询并应用维度过滤
        var hourlyUsageQuery = context.AppHourlyUsages.AsNoTracking()
            .Where(h => startTime <= h.Hour && h.Hour < endTime);

        var logQuery = context.ActivityLogs.AsNoTracking()
            .Where(a => startTime <= a.LoggedAt && a.LoggedAt < endTime);

        if (isApp)
        {
            hourlyUsageQuery = hourlyUsageQuery.Where(h => h.AppId == request.Id);
            logQuery = logQuery.Where(a => a.AppId == request.Id);
        }
        else
        {
            // 维度为 category 时，通过导航属性过滤
            hourlyUsageQuery = hourlyUsageQuery.Where(h => h.App!.AppCategoryId == request.Id);
            logQuery = logQuery.Where(a => a.App!.AppCategoryId == request.Id);
        }

        // 2. 投影、合并并执行聚合
        var usageByHour = await hourlyUsageQuery
            .Select(h => new { Hour = h.Hour.Hour, h.DurationMilliseconds })
            .Concat(logQuery.Select(a => new { Hour = a.LoggedAt.Hour, a.DurationMilliseconds }))
            .GroupBy(u => u.Hour)
            .Select(g => new
            {
                Hour = g.Key,
                DurationMilliseconds = g.Sum(u => u.DurationMilliseconds)
            })
            .ToDictionaryAsync(
                h => h.Hour,
                h => h.DurationMilliseconds,
                cancellationToken
            );

        // 3. 生成 0-23 小时的结果并补齐 0
        return [.. Enumerable.Range(0, 24)
            .Select(hour => new GetUsageByHourResponseItem(
                Hour: hour,
                DurationSeconds: usageByHour.GetValueOrDefault(hour) / 1000
            ))];
    }
}