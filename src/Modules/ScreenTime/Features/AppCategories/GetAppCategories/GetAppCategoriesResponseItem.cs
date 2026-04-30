namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategories;

public record GetAppCategoriesResponseItem(
    Guid Id,
    string Name,
    string? IconPath,
    bool IsSystem
);