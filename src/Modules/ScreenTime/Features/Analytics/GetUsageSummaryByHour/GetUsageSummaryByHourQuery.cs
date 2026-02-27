using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Analytics.GetUsageSummaryByHour;

public record GetUsageSummaryByHourQuery(
    string Dimension,
    DateOnly Date,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetUsageSummaryByHourResponseItem>>;
