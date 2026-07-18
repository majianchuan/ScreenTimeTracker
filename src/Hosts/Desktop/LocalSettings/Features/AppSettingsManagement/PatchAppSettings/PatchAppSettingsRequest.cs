using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.PatchAppSettings;

public record PatchAppSettingsRequest(
    UIOpenMode? DefaultUIOpenMode,
    bool? IsAutoStartEnabled,
    bool? IsSilentStartEnabled,
    string? Language
);
