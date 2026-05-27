namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public record GetAppSettingsResult(
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