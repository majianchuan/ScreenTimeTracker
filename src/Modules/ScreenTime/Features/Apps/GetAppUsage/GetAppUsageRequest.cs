using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public record GetAppUsageRequest(
    [property: QueryParam] string Granularity,
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate,
    [property: QueryParam] IEnumerable<Guid>? IncludedIds = null,
    [property: QueryParam] IEnumerable<Guid>? ExcludedIds = null
);