using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppUsageDistribution;

public record GetAppUsageDistributionQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    int TopN = 10,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<GetAppUsageDistributionResponse>;
