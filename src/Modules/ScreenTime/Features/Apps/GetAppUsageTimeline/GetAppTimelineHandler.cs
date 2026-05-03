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
        var excludedIds = request.ExcludedIds?.ToList() ?? [];

        var sessions = await context.AppUsageSessions
            .AsNoTracking()
            .Where(x => !excludedIds.Contains(x.AppId)
                && x.StartTime < endTime
                && startTime <= x.EndTime)
            .Select(x => new
            {
                Id = x.AppId,
                x.App!.Name,
                x.StartTime,
                x.EndTime
            })
            .ToListAsync(cancellationToken);

        var activeSession = activeSessionStore.Current;
        if (activeSession is not null
            && !excludedIds.Contains(activeSession.AppId)
            && activeSession.StartTime < endTime)
        {
            var activeApp = await context.Apps
                .AsNoTracking()
                .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);
            sessions.Add(new
            {
                activeApp.Id,
                activeApp!.Name,
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
            .Select(x => new GetAppUsageTimelineResponseItem(
                Id: x.Id,
                Name: x.Name,
                StartTime: x.StartTime.ToString("O"),
                EndTime: x.EndTime.ToString("O")
            ))];
    }
}
