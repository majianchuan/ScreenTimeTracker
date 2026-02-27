namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsResult(
    string UIOpenMode,
    bool AutoStart,
    bool SilentStart,
    string Language,
    bool WindowDestroyOnClose
);