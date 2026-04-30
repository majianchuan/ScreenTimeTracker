using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsage;

public record GetAppCategoryUsageRequest(
    [property: QueryParam] string Granularity,
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate,
    [property: QueryParam] IEnumerable<Guid>? IncludedIds = null,
    [property: QueryParam] IEnumerable<Guid>? ExcludedIds = null
);