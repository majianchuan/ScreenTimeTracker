namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public record CreateAppCategoryResponse(
    Guid Id,
    string Name,
    string? IconPath,
    bool IsSystem
);