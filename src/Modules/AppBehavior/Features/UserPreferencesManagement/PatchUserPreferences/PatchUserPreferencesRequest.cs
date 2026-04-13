namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;

public record PatchUserPreferencesRequest(
    string? UIOpenMode,
    bool? AutoStart,
    bool? SilentStart,
    string? Language,
    bool? WindowDestroyOnClose
);
