using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserPreferencesManagement.PatchUserSettings;

public record PatchUserSettingsCommand(
    TimeSpan? SamplingInterval,
    bool? IdleDetection,
    TimeSpan? IdleTimeout,
    TimeSpan? AppInfoStaleThreshold,
    TimeSpan? AggregationInterval,
    string? AppIconDirectory
) : IRequest
{ }