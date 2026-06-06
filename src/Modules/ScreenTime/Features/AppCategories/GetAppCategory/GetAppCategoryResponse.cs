namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategory;

public record GetAppCategoryResponse(
    Guid Id,
    string Name,
    string Color,
    string? IconPath,
    bool IsSystem
);