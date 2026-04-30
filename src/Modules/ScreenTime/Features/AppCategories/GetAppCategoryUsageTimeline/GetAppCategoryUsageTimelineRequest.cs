using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageTimeline.GetAppCategoryUsageTimeline;

public record GetAppCategoryUsageTimelineRequest(
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate,
    [property: QueryParam] IEnumerable<Guid>? ExcludedIds = null
);