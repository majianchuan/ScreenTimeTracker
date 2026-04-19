namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;

public record PatchUserPreferencesRequest(
    string? DefaultUIOpenMode,
    bool? IsAutoStartEnabled,
    bool? IsSilentStartEnabled,
    string? Language,
    bool? ShouldDestroyWindowOnClose
);
