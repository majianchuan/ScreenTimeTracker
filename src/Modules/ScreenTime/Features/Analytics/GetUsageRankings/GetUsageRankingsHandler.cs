using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageRankings;

public class GetUsageRankingsHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetUsageRankingsQuery, List<GetUsageRankingsResponseItem>>
{
    public async ValueTask<List<GetUsageRankingsResponseItem>> Handle(GetUsageRankingsQuery request, CancellationToken cancellationToken)
    {
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1);
        var isApp = request.Dimension == "app";

        // 1. 构建基础查询
        var hourlyQuery = context.AppHourlyUsages.AsNoTracking().Where(h => startTime <= h.Hour && h.Hour < endTime);
        var logQuery = context.ActivityLogs.AsNoTracking().Where(l => startTime <= l.LoggedAt && l.LoggedAt < endTime);

        // 2. 统一过滤和投影 (根据维度选择 ID)
        // 使用表达式定义如何获取 ID，避免重复的 If-Else
        var combinedQuery = hourlyQuery.Select(h => new { Id = isApp ? h.AppId : h.App!.AppCategoryId, h.DurationMilliseconds })
            .Concat(logQuery.Select(l => new { Id = isApp ? l.AppId : l.App!.AppCategoryId, l.DurationMilliseconds }));

        // 3. 应用排除逻辑并聚合
        if (request.ExcludedIds?.Any() is true)
            combinedQuery = combinedQuery.Where(x => !request.ExcludedIds.Contains(x.Id));

        var usageSummary = await combinedQuery
            .GroupBy(x => x.Id)
            .Select(g => new { Id = g.Key, TotalMs = g.Sum(x => x.DurationMilliseconds) })
            .ToListAsync(cancellationToken);

        if (usageSummary.Count == 0) return [];

        // 4. 计算总计并取 Top N
        var totalMs = (double)usageSummary.Sum(x => x.TotalMs);
        var topUsage = usageSummary.OrderByDescending(x => x.TotalMs).Take(request.TopN).ToList();
        var topIds = topUsage.Select(x => x.Id).ToList();

        // 5. 统一获取元数据 (Name, Icon)
        // 根据维度决定查哪张表，投影到相同的结构
        var metadataQuery = isApp
            ? context.Apps.Where(a => topIds.Contains(a.Id)).Select(a => new { a.Id, a.Name, a.IconPath })
            : context.AppCategories.Where(c => topIds.Contains(c.Id)).Select(c => new { c.Id, c.Name, c.IconPath });

        var metadataDict = await metadataQuery.ToDictionaryAsync(x => x.Id, cancellationToken);

        // 6. 组装结果
        return [.. topUsage
            .Where(u => metadataDict.ContainsKey(u.Id)) // 确保元数据存在
            .Select(u => new GetUsageRankingsResponseItem(
                Id: u.Id,
                Name: metadataDict[u.Id].Name,
                IconPath: metadataDict[u.Id].IconPath,
                DurationSeconds: u.TotalMs / 1000,
                Percentage: (int)(u.TotalMs * 100 / totalMs)
            ))];
    }
}