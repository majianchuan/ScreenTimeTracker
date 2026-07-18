using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public record GetAppUsageQuery(
    UsageGranularity Granularity,
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? IncludedIds = null,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppUsageResponseItem>>;

public enum UsageGranularity
{
    Hour,
    Day
}