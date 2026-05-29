using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UsageTimeline.GetAppCategoryUsageTimeline;

public record GetAppCategoryUsageTimelineQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? IncludedIds = null,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppCategoryUsageTimelineResponseItem>>;
