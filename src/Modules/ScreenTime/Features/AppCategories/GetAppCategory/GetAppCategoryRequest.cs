using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategory;

public record GetAppCategoryRequest(
    [property: RouteParam] Guid AppCategoryId
);