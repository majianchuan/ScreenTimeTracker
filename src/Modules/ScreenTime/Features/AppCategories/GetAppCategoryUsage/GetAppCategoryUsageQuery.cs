using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsage;

public record GetAppCategoryUsageQuery(
    UsageGranularity Granularity,
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? IncludedIds = null,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppCategoryUsageResponseItem>>;

public enum UsageGranularity
{
    Hour,
    Day
}