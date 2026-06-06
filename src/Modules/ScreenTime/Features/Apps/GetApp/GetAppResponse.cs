namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApp;

public record GetAppResponse(
    Guid Id,
    string Name,
    string Color,
    string ProcessName,
    bool IsAutoUpdateEnabled,
    string LastAutoUpdated,
    Guid AppCategoryId,
    string? ExecutablePath,
    string? IconPath,
    bool IsSystem
);