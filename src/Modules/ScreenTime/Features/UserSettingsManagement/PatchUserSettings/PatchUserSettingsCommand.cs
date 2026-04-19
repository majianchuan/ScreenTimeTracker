using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.PatchUserSettings;

public record PatchUserSettingsCommand(
    string? AppIconDirectory,
    TimeSpan? AppInfoStaleThreshold,
    TimeSpan? ActiveSessionAutoSaveInterval,

    bool? IsIdleDetectionEnabled,
    TimeSpan? IdleThreshold,
    TimeSpan? IdleDetectionPollingInterval,

    TimeSpan? MinValidSessionDuration,
    TimeSpan? SessionMergeTolerance,
    TimeSpan? SessionOptimizationInterval,

    int? DayBoundaryOffsetHours
) : IRequest
{ }