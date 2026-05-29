using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageTimeline;

public class GetAppUsageTimelineHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider
    ) : IRequestHandler<GetAppUsageTimelineQuery, List<GetAppUsageTimelineResponseItem>>
{
    public async ValueTask<List<GetAppUsageTimelineResponseItem>> Handle(GetAppUsageTimelineQuery request, CancellationToken cancellationToken)
    {
        var settings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);
        var startTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddHours(settings.DayCutoffHour);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1).AddHours(settings.DayCutoffHour);

        var query = context.AppUsageSessions
            .AsNoTracking()
            .Where(x => x.StartTime < endTime && startTime <= x.EndTime);

        if (request.IncludedIds is not null)
            query = query.Where(x => request.IncludedIds.Contains(x.AppId));
        else if (request.ExcludedIds is not null)
            query = query.Where(x => !request.ExcludedIds.Contains(x.AppId));

        var sessions = await query
            .Select(x => new
            {
                Id = x.AppId,
                x.App!.Name,
                x.StartTime,
                x.EndTime
            })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null && activeSession.StartTime < endTime)
        {
            var included = request.IncludedIds?.Contains(activeSession.AppId) ?? true;
            var notExcluded = request.ExcludedIds is null || !request.ExcludedIds.Contains(activeSession.AppId);
            var shouldInclude = request.IncludedIds is not null ? included : notExcluded;

            if (shouldInclude)
            {
                var activeApp = await context.Apps
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);
                sessions.Add(new
                {
                    activeApp.Id,
                    activeApp.Name,
                    activeSession.StartTime,
                    EndTime = timeProvider.GetLocalNow().DateTime
                });
            }
        }

        var normalized = sessions.Select(s => s with
        {
            StartTime = s.StartTime < startTime ? startTime : s.StartTime,
            EndTime = endTime < s.EndTime ? endTime : s.EndTime
        });

        return [.. normalized
            .OrderBy(x => x.StartTime)
            .Select(x => new GetAppUsageTimelineResponseItem(
                Id: x.Id,
                Name: x.Name,
                StartTime: x.StartTime.ToString("O"),
                EndTime: x.EndTime.ToString("O")
            ))];
    }
}
