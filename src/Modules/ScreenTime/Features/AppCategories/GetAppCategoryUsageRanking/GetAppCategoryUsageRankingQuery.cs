using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageRanking.GetAppCategoryUsageRanking;

public record GetAppCategoryUsageRankingQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    int TopN = 10,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppCategoryUsageRankingResponseItem>>;
