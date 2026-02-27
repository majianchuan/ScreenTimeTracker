namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public record CreateAppCategoryRequest(
    string Name,
    string? IconPath
);