namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.PatchUserSettings;

public record PatchUserSettingsRequest(
    string? UIOpenMode,
    bool? AutoStart,
    bool? SilentStart,
    string? Language,
    bool? WindowDestroyOnClose
);
