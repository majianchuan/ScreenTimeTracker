namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApp;

public record GetAppResponse(
    Guid Id,
    string Name,
    string ProcessName,
    bool IsAutoUpdateEnabled,
    string LastAutoUpdated,
    Guid AppCategoryId,
    string? ExecutablePath,
    string? IconPath,
    string? Description,
    bool IsSystem
);