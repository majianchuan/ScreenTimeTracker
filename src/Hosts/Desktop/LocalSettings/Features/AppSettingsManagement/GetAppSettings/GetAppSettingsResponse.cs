namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public record GetAppSettingsResponse(
    string DefaultUIOpenMode,
    bool IsAutoStartEnabled,
    bool IsSilentStartEnabled,
    string Language,
    bool ShouldDestroyWindowOnClose
);
