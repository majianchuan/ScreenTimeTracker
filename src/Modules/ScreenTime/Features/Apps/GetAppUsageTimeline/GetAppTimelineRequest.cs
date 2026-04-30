using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageTimeline;

public record GetAppUsageTimelineRequest(
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate,
    [property: QueryParam] IEnumerable<Guid>? ExcludedIds = null
);