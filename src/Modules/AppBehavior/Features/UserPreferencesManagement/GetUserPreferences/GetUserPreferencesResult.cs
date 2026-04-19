namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

public record GetUserPreferencesResult(
    UIOpenModeDto DefaultUIOpenMode,
    bool IsAutoStartEnabled,
    bool IsSilentStartEnabled,
    string Language,
    bool ShouldDestroyWindowOnClose
);

public enum UIOpenModeDto
{
    Window,
    Browser
}