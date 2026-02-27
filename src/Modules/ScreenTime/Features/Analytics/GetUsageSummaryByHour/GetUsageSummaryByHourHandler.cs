using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByHour;

public class GetUsageSummaryByHourHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetUsageSummaryByHourQuery, List<GetUsageSummaryByHourResponseItem>>
{
    public async ValueTask<List<GetUsageSummaryByHourResponseItem>> Handle(GetUsageSummaryByHourQuery request, CancellationToken cancellationToken)
    {
        var startTime = request.Date.ToDateTime(TimeOnly.MinValue);
        var endTime = startTime.AddDays(1);

        // 1. 构建基础查询（仅时间过滤）
        var hourlyQuery = context.AppHourlyUsages.AsNoTracking().Where(h => startTime <= h.Hour && h.Hour < endTime);
        var logQuery = context.ActivityLogs.AsNoTracking().Where(l => startTime <= l.LoggedAt && l.LoggedAt < endTime);

        // 2. 应用维度排除过滤
        if (request.ExcludedIds?.Any() is true)
        {
            bool isApp = request.Dimension == "app";
            // 利用 EF Core 自动处理 Join 的特性，直接访问导航属性，移除 redundant 的 Include
            hourlyQuery = isApp ? hourlyQuery.Where(h => !request.ExcludedIds.Contains(h.AppId))
                                : hourlyQuery.Where(h => !request.ExcludedIds.Contains(h.App!.AppCategoryId));

            logQuery = isApp ? logQuery.Where(l => !request.ExcludedIds.Contains(l.AppId))
                             : logQuery.Where(l => !request.ExcludedIds.Contains(l.App!.AppCategoryId));
        }

        // 3. 聚合数据到字典
        var usageByHour = await hourlyQuery
            .Select(h => new { Hour = h.Hour.Hour, h.DurationMilliseconds })
            .Concat(logQuery.Select(l => new { Hour = l.LoggedAt.Hour, l.DurationMilliseconds }))
            .GroupBy(x => x.Hour)
            .Select(g => new { Hour = g.Key, TotalMs = g.Sum(x => x.DurationMilliseconds) })
            .ToDictionaryAsync(x => x.Hour, x => x.TotalMs, cancellationToken);

        // 4. 生成 0-23 小时的结果并补全 0
        return [.. Enumerable.Range(0, 24)
            .Select(h => new GetUsageSummaryByHourResponseItem(
                Hour: h,
                DurationSeconds: usageByHour.GetValueOrDefault(h) / 1000
            ))];
    }
}