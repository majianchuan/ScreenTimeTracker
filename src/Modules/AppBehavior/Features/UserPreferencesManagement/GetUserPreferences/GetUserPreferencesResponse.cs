namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

public record GetUserPreferencesResponse(
    string UIOpenMode,
    bool AutoStart,
    bool SilentStart,
    string Language,
    bool WindowDestroyOnClose
);
