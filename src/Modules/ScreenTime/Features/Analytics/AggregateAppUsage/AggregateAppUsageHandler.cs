using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.AggregateAppUsage;

public class AggregateAppUsageHandler(
    ScreenTimeDbContext context,
    TimeProvider timeProvider,
    ILogger<AggregateAppUsageHandler> logger
    ) : IRequestHandler<AggregateAppUsageCommand>
{
    public async ValueTask<Unit> Handle(AggregateAppUsageCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetLocalNow().DateTime;
        UserSettings userSettings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        DateTime freezeLimit = now.Add(-userSettings.IdleTimeout);
        // 规整到小时
        DateTime dataFreezeTime = new(freezeLimit.Year, freezeLimit.Month, freezeLimit.Day, freezeLimit.Hour, 0, 0);

        // 开启显式事务，保证聚合和删除的原子性
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 聚合dataFreezeTime以前的数据
            var rawSummaries = await context.ActivityLogs
                .AsNoTracking()
                .Where(t => t.LoggedAt < dataFreezeTime)
                .GroupBy(t => new
                {
                    t.AppId,
                    Hour = new DateTime(t.LoggedAt.Year, t.LoggedAt.Month, t.LoggedAt.Day, t.LoggedAt.Hour, 0, 0)
                })
                .Select(g => new
                {
                    AppId = g.Key.AppId,
                    Hour = g.Key.Hour,
                    DurationMilliseconds = g.Sum(x => x.DurationMilliseconds)
                })
                .ToListAsync(cancellationToken);

            if (rawSummaries.Count == 0)
                return Unit.Value;

            // 一次性查出所有涉及到的现有 Summary
            var affectedAppIds = rawSummaries.Select(x => x.AppId).Distinct().ToList();
            var affectedHours = rawSummaries.Select(x => x.Hour).Distinct().ToList();

            var existingSummaries = await context.AppHourlyUsages
                .Where(s => affectedAppIds.Contains(s.AppId) && affectedHours.Contains(s.Hour))
                .ToDictionaryAsync(
                    k => (k.AppId, k.Hour),
                    v => v,
                    cancellationToken);

            // 内存中合并逻辑
            foreach (var raw in rawSummaries)
            {
                var key = (raw.AppId, raw.Hour);
                var duration = TimeSpan.FromMilliseconds(raw.DurationMilliseconds);

                if (existingSummaries.TryGetValue(key, out var existing))
                {
                    existing.Accumulate(duration);
                }
                else
                {
                    var newSummary = AppHourlyUsage.Create(raw.AppId, raw.Hour, duration);
                    context.AppHourlyUsages.Add(newSummary);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            // 批量删除dataFreezeTime以前的数据
            await context.ActivityLogs
                .Where(t => t.LoggedAt < dataFreezeTime)
                .ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SummarizeHourlyData failed.");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return Unit.Value;
    }
}
