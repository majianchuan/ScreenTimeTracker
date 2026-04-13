namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

public record GetUserPreferencesResult(
    UIOpenModeDto UIOpenMode,
    bool AutoStart,
    bool SilentStart,
    string Language,
    bool WindowDestroyOnClose
);

public enum UIOpenModeDto
{
    Window,
    Browser
}