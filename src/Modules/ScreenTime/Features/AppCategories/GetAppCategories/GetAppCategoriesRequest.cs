using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategories;

public record GetAppCategoriesRequest(
    [property: QueryParam] string? Fields
);