namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsResult(
    string AppIconDirectory,
    TimeSpan AppInfoStaleThreshold,
    TimeSpan ActiveSessionAutoSaveInterval,

    bool IsIdleDetectionEnabled,
    TimeSpan IdleThreshold,
    TimeSpan IdleDetectionPollingInterval,

    TimeSpan MinValidSessionDuration,
    TimeSpan SessionMergeTolerance,
    TimeSpan SessionOptimizationInterval,

    int DayCutoffHour
);
