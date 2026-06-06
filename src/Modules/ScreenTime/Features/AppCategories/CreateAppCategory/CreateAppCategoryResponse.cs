namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public record CreateAppCategoryResponse(
    Guid Id,
    string Name,
    string Color,
    string? IconPath,
    bool IsSystem
);