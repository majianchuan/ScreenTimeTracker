namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.PatchAppSettings;

public record PatchAppSettingsRequest(
    string? DefaultUIOpenMode,
    bool? IsAutoStartEnabled,
    bool? IsSilentStartEnabled,
    string? Language,
    bool? ShouldDestroyWindowOnClose
);
