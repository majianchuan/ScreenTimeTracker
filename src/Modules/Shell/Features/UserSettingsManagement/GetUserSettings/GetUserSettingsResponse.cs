namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsResponse(
    string UIOpenMode,
    bool AutoStart,
    bool SilentStart,
    string Language,
    bool WindowDestroyOnClose
);
