namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApp;

public record GetAppResponse(
    Guid Id,
    string Name,
    string Color,
    string ProcessName,
    bool IsAutoUpdateEnabled,
    DateTime LastAutoUpdatedAt,
    Guid AppCategoryId,
    string? ExecutablePath,
    string? IconPath,
    DateTime IconLastUpdatedAt,
    bool IsSystem
);