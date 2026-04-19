using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageStats.GetUsageSummaryByHour;

public record GetUsageSummaryByHourQuery(
    string Dimension,
    DateOnly Date,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetUsageSummaryByHourResponseItem>>;
