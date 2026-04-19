using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByDay;

public record GetUsageSummaryByDayRequest(
    [property: QueryParam] string Dimension,
    [property: QueryParam, BindFrom("start-date")] DateOnly StartDate,
    [property: QueryParam, BindFrom("end-date")] DateOnly EndDate,
    [property: QueryParam, BindFrom("excluded-ids")] IEnumerable<Guid>? ExcludedIds = null
);