using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsage;

public record GetAppUsageQuery(
    string Granularity,
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? IncludedIds = null,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppUsageResponseItem>>;
