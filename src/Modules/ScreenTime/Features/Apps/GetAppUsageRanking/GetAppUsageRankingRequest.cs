using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageRanking;

public record GetAppUsageRankingRequest(
    [property: QueryParam] DateOnly StartDate,
    [property: QueryParam] DateOnly EndDate,
    [property: QueryParam] int TopN = 10,
    [property: QueryParam] IEnumerable<Guid>? ExcludedIds = null
);
