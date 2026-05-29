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

        var query = context.AppUsageSessions
            .AsNoTracking()
            .Where(x => x.StartTime < endTime && startTime <= x.EndTime);

        if (request.IncludedIds is not null)
            query = query.Where(x => request.IncludedIds.Contains(x.App!.AppCategoryId));
        else if (request.ExcludedIds is not null)
            query = query.Where(x => !request.ExcludedIds.Contains(x.App!.AppCategoryId));

        var sessions = await query
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
                .AsNoTracking()
                .SingleAsync(x => x.Id == activeSession.AppId, cancellationToken);
            var included = request.IncludedIds?.Contains(activeApp.AppCategoryId) ?? true;
            var notExcluded = request.ExcludedIds is null || !request.ExcludedIds.Contains(activeApp.AppCategoryId);
            var shouldInclude = request.IncludedIds is not null ? included : notExcluded;

            if (shouldInclude)
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
            .Select(x => new GetAppCategoryUsageTimelineResponseItem(
                Id: x.Id,
                Name: x.Name,
                StartTime: x.StartTime.ToString("O"),
                EndTime: x.EndTime.ToString("O")
            ))];
    }
}
