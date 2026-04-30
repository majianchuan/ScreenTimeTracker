using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageTimeline;

public record GetAppUsageTimelineQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppUsageTimelineResponseItem>>;
