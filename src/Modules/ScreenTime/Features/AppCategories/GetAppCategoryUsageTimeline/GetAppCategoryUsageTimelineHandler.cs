using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageTimeline.GetAppCategoryUsageTimeline;

public class GetAppCategoryUsageTimelineHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetAppCategoryUsageTimelineQuery, List<GetAppCategoryUsageTimelineResponseItem>>
{
    public async ValueTask<List<GetAppCategoryUsageTimelineResponseItem>> Handle(GetAppCategoryUsageTimelineQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayCutoffHour);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayCutoffHour);
        var excludedIds = request.ExcludedIds?.ToList() ?? [];

        var sessions = await context.AppUsageSessions
            .AsNoTracking()
            .Where(x =>
                !excludedIds.Contains(x.App!.AppCategoryId)
                && x.StartTime < endTime
                && startTime <= x.EndTime)
            .Select(x => new
            {
                Id = x.App!.AppCategoryId,
                x.App!.AppCategory!.Name,
                x.StartTime,
                x.EndTime
            })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null && activeSession.StartTime < endTime)
        {
            var activeApp = await context.Apps
                .Include(x => x.AppCategory)
                .AsNoTracking()
                .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);

            if (!excludedIds.Contains(activeApp.AppCategoryId))
                sessions.Add(new
                {
                    Id = activeApp.AppCategoryId,
                    activeApp.AppCategory!.Name,
                    activeSession.StartTime,
                    EndTime = timeProvider.GetLocalNow().DateTime
                });
        }

        var normalized = sessions.Select(s => s with
        {
            StartTime = s.StartTime < startTime ? startTime : s.StartTime,
            EndTime = endTime < s.EndTime ? endTime : s.EndTime
        });

        return [.. normalized
            .OrderBy(x => x.StartTime)
            .Select(x => new GetAppCategoryUsageTimelineResponseItem(
                Id: x.Id,
                Name: x.Name,
                StartTime: x.StartTime.ToString("O"),
                EndTime: x.EndTime.ToString("O")
            ))];
    }
}
