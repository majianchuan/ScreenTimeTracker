using Mediator;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;

public record PatchUserPreferencesCommand(
    string? DefaultUIOpenMode,
    bool? IsAutoStartEnabled,
    bool? IsSilentStartEnabled,
    string? Language,
    bool? ShouldDestroyWindowOnClose
) : IRequest;

