using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryIcon;

public record GetAppCategoryIconPathQuery(
    Guid Id
) : IRequest<string?>;
