using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageRankings;

public record GetUsageRankingsRequest(
    [property: QueryParam] string Dimension,
    [property: QueryParam, BindFrom("start-date")] DateOnly StartDate,
    [property: QueryParam, BindFrom("end-date")] DateOnly EndDate,
    [property: QueryParam, BindFrom("top-n")] int TopN = 10,
    [property: QueryParam, BindFrom("excluded-ids")] IEnumerable<Guid>? ExcludedIds = null
);
