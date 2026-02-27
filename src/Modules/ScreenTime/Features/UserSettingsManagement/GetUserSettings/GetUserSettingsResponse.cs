namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsResponse(
    int SamplingIntervalMilliseconds,
    bool IdleDetection,
    int IdleTimeoutSeconds,
    int AppInfoStaleThresholdMinutes,
    int AggregationIntervalMinutes,
    string AppIconDirectory
);