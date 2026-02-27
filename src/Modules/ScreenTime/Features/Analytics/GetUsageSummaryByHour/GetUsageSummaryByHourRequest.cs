using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByHour;

public record GetUsageSummaryByHourRequest(
    [property: QueryParam] string Dimension,
    [property: QueryParam] DateOnly Date,
    [property: QueryParam, BindFrom("excluded-ids")] IEnumerable<Guid>? ExcludedIds = null
);