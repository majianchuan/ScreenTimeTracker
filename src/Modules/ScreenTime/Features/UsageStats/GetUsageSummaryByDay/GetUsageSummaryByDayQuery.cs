using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByDay;

public record GetUsageSummaryByDayQuery(
    string Dimension,
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetUsageSummaryByDayResponseItem>>;
