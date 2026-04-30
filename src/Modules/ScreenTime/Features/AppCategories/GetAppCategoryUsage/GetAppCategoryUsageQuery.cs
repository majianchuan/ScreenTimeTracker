using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryUsage;

public record GetAppCategoryUsageQuery(
    string Granularity,
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<Guid>? IncludedIds = null,
    IEnumerable<Guid>? ExcludedIds = null
) : IRequest<List<GetAppCategoryUsageResponseItem>>;
