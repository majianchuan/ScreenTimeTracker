using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryIcon;

public record GetAppCategoryIconRequest(
    [property: RouteParam] Guid AppCategoryId
);