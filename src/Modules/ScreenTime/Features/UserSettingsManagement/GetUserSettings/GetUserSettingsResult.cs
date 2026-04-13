namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserPreferencesManagement.GetUserSettings;

public record GetUserSettingsResult(
    TimeSpan SamplingInterval,
    bool IdleDetection,
    TimeSpan IdleTimeout,
    TimeSpan AppInfoStaleThreshold,
    TimeSpan AggregationInterval,
    string AppIconDirectory
);
