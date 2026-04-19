namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsResponse(
    string AppIconDirectory,
    int AppInfoStaleThresholdMinutes,
    int ActiveSessionAutoSaveSeconds,

    bool IsIdleDetectionEnabled,
    int IdleThresholdSeconds,
    int IdleDetectionPollingIntervalSeconds,

    int MinValidSessionDurationSeconds,
    int SessionMergeToleranceSeconds,
    int SessionOptimizationIntervalSeconds,

    int DayBoundaryOffsetHours
);
