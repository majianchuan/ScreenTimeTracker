namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserPreferencesManagement.PatchUserSettings;

public record PatchUserSettingsRequest(
    int? SamplingIntervalMilliseconds,
    bool? IdleDetection,
    int? IdleTimeoutSeconds,
    int? AppInfoStaleThresholdMinutes,
    int? AggregationIntervalMinutes,
    string? AppIconDirectory
);