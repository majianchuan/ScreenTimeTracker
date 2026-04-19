using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageRankings;

public record GetUsageRankingsQuery(
    string Dimension,
    DateOnly StartDate,
    DateOnly EndDate,
    int TopN = 10,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetUsageRankingsResponseItem>>;
