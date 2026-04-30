namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.PatchUserSettings;

public record PatchUserSettingsRequest(
    string? AppIconDirectory,
    int? AppInfoStaleThresholdMinutes,
    int? ActiveSessionAutoSaveSeconds,

    bool? IsIdleDetectionEnabled,
    int? IdleThresholdSeconds,
    int? IdleDetectionPollingIntervalSeconds,

    int? MinValidSessionDurationSeconds,
    int? SessionMergeToleranceSeconds,
    int? SessionOptimizationIntervalMinutes,

    int? DayCutoffHour
);