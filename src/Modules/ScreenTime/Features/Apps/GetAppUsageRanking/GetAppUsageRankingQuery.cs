using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageRanking;

public record GetAppUsageRankingQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    int TopN = 10,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppUsageRankingResponseItem>>;
