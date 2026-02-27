namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApps;

public record GetAppsResponseItem(
    Guid Id,
    string Name,
    string ProcessName,
    bool IsAutoUpdateEnabled,
    string LastAutoUpdated,
    Guid AppCategoryId,
    string? ExecutablePath,
    string IconPath,
    string? Description
);