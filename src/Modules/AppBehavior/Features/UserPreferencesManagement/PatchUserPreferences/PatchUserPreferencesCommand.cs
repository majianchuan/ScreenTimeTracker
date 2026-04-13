using Mediator;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;

public record PatchUserPreferencesCommand(
    string? UIOpenMode,
    bool? AutoStart,
    bool? SilentStart,
    string? Language,
    bool? WindowDestroyOnClose
) : IRequest;

