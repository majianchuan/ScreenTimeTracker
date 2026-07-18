using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsageDistribution;

public record GetAppCategoryUsageDistributionQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    int TopN = 10,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<GetAppCategoryUsageDistributionResponse>;
